using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;
using System.Collections.Concurrent;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Common.Core.Helpers;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Newtonsoft.Json;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.InternalSvc {
    public class InternalOrderHistoryLogic : IInternalOrderHistoryLogic {
        #region attributes
		private const int RECORDTYPE_LENGTH = 1;
		private const int RECORDTYPE_STARTPOS = 0;
		private const int THREAD_SLEEP_DURATION = 2000;


        
		private readonly IOrderHistoryHeaderRepsitory _headerRepo;
		private readonly IPurchaseOrderRepository _poRepo;
		private readonly IKPayInvoiceRepository _kpayInvoiceRepository;
		private readonly ICatalogLogic _catalogLogic;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEventLogRepository _log;
		private readonly IGenericQueueRepository _queue;
		private readonly IOrderConversionLogic _conversionLogic;
		private readonly ICustomerRepository _customerRepository;

		private bool _keepListening;
		private Task _queueTask;
        #endregion

        #region ctor
		public InternalOrderHistoryLogic(IOrderHistoryHeaderRepsitory headerRepo,
			IPurchaseOrderRepository poRepo, IKPayInvoiceRepository kpayInvoiceRepository, ICatalogLogic catalogLogic,
			IUnitOfWork unitOfWork, IEventLogRepository log, IGenericQueueRepository queue, IOrderConversionLogic conversionLogic, ICustomerRepository customerRepository)
		{
			_headerRepo = headerRepo;
			_poRepo = poRepo;
			_kpayInvoiceRepository = kpayInvoiceRepository;
			_catalogLogic = catalogLogic;
			_unitOfWork = unitOfWork;
			_log = log;
			_queue = queue;
			_conversionLogic = conversionLogic;
			_keepListening = true;
			_customerRepository = customerRepository;
        }
        #endregion

        #region methods
        public Order GetOrder(string branchId, string invoiceNumber) {

			EF.OrderHistoryHeader myOrder = _headerRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
																  h.InvoiceNumber.Equals(invoiceNumber),
															 d => d.OrderDetails).FirstOrDefault();
            PurchaseOrder po = null;
			
            Order returnOrder = null;
				
			if (myOrder == null) {
                po = _poRepo.ReadPurchaseOrderByTrackingNumber(invoiceNumber);
				returnOrder = po.ToOrder();
			} else {
				returnOrder = myOrder.ToOrder();

				if (myOrder.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && myOrder.ControlNumber.Length > 0)
				{
					po = _poRepo.ReadPurchaseOrderByTrackingNumber(myOrder.ControlNumber);
					if (po != null) {
						returnOrder.Status = po.Status;
					}

					if (myOrder.ActualDeliveryTime != null) {
						returnOrder.Status = "Delivered";
					}
				}
			}

			LookupProductDetails(branchId, returnOrder);

            if (po != null) {
                returnOrder.IsChangeOrderAllowed = (po.Properties["MasterNumber"] != null && (po.Status.StartsWith("Confirmed")));
            }

			if (returnOrder.Status == "Submitted" && returnOrder.Items != null) {
				//Set all item status' to Pending. This is kind of a hack, but the correct fix will require more effort than available at the moment. The Status/Mainframe status changes are what's causing this issue
				foreach (var item in returnOrder.Items)
					item.MainFrameStatus = "Pending";
			}

			returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);

			return returnOrder;
        }

        public List<Order> GetOrders(Guid userId, UserSelectedContext customerInfo) {
			return GetOrderHistoryOrders(customerInfo).OrderByDescending(o => o.InvoiceNumber).ToList<Order>();
        }
		
		public List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate){
			var customer = _customerRepository.GetCustomerByCustomerNumber(customerInfo.CustomerId, customerInfo.BranchId);

			var oh = GetOrderHistoryHeadersForDateRange(customerInfo, startDate, endDate);
			var cs = _poRepo.ReadPurchaseOrderHeadersInDateRange(customer.CustomerId, customerInfo.CustomerId, startDate, endDate);

			return MergeOrderLists(cs.Select(o => o.ToOrder()).ToList(), oh);
		}

        public void SaveOrder(OrderHistoryFile historyFile) {
			Create(historyFile);

			_unitOfWork.SaveChanges();
        }

		public Core.Models.Paging.PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, Core.Models.Paging.PagingModel paging)
		{
			var headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
																			  h.CustomerNumber.Equals(customerInfo.CustomerId),
																			d => d.OrderDetails);

			return LookupControlNumberAndStatus(customerInfo, headers).AsQueryable().GetPage(paging);
		}

		public void StopListening()
		{
			_keepListening = false;

			if (_queueTask != null && _queueTask.Status == TaskStatus.Running)
			{
				_queueTask.Wait();
			}
		}
        
		public void ListenForQueueMessages()
		{
			_queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
		}

		private void ListenForQueueMessagesInTask()
		{
			while (_keepListening)
			{
				System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

				try
				{
					var rawOrder = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);

					while (!string.IsNullOrEmpty(rawOrder))
					{
						OrderHistoryFile historyFile = new OrderHistoryFile();

						historyFile = JsonConvert.DeserializeObject<OrderHistoryFile>(rawOrder);

						_log.WriteInformationLog(string.Format("Consuming order update from queue for message ({0})", historyFile.MessageId));

						Create(historyFile);
						_conversionLogic.SaveOrderHistoryAsConfirmation(historyFile);

						_unitOfWork.SaveChangesAndClearContext();

						rawOrder = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);
												
					}
										
				}
				catch (Exception ex)
				{
					KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, subject: "Exception processing Order History in Queue Service");

					_log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
				}
			}

		}

		private void Create(OrderHistoryFile currentFile)
		{
			// first attempt to find the order, look by branch and invoice number
			EF.OrderHistoryHeader header = _headerRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber).FirstOrDefault();

			// second attempt to find the order, look by confirmation number
			if (header == null && !string.IsNullOrEmpty(currentFile.Header.ControlNumber)) { header = _headerRepo.ReadByConfirmationNumber(currentFile.Header.ControlNumber).FirstOrDefault(); }

			// last ditch effort is to create a new header
			if (header == null)
			{
				header = new EF.OrderHistoryHeader();
				header.OrderDetails = new List<EF.OrderHistoryDetail>();
			}

			currentFile.Header.MergeWithEntity(ref header);

			foreach (OrderHistoryDetail currentDetail in currentFile.Details.ToList())
			{

                EF.OrderHistoryDetail detail = null;

                if (header.OrderDetails != null && header.OrderDetails.Count > 0) {
                    detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();
                }

                if (detail == null)
				{
					EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
					tempDetail.BranchId = header.BranchId;
					tempDetail.InvoiceNumber = header.InvoiceNumber;

					header.OrderDetails.Add(tempDetail);
				}
				else
				{
					currentDetail.MergeWithEntityFrameworkModel(ref detail);

					detail.BranchId = header.BranchId;
					detail.InvoiceNumber = header.InvoiceNumber;
				}
			}

			_headerRepo.CreateOrUpdate(header);
		}
		
		private List<Order> LookupControlNumberAndStatus(UserSelectedContext userContext, IEnumerable<EF.OrderHistoryHeader> headers)
		{
			var customerOrders = new BlockingCollection<Order>();
			foreach (var h in headers)
			{
                try
                {
                    Order returnOrder = null;

                    returnOrder = h.ToOrder();

                    if (h.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && h.ControlNumber.Length > 0)
                    {
                        var po = _poRepo.ReadPurchaseOrderByTrackingNumber(h.ControlNumber);
                        if (po != null)
                        {
                            returnOrder.Status = po.Status;
                            returnOrder.OrderNumber = h.ControlNumber;
							returnOrder.IsChangeOrderAllowed = (po.Properties["MasterNumber"] != null && (po.Status.StartsWith("Confirmed")));

							//if (po.Properties["LineItems"] != null)
							//{
							//	var poOrder = po.ToOrder();
							//	foreach (var item in returnOrder.Items)
							//	{
							//		//Get the unit price from the PO
							//		var poLine = poOrder.Items.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
							//		if (poLine != null)
							//			item.Price = poLine.Price;
							//	}
							//}
                        }

						var invoice = _kpayInvoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, returnOrder.InvoiceNumber);
						if (invoice != null)
						{
							returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
							
						}

                    }

                    if (returnOrder.ActualDeliveryTime != null)
                    {
                        returnOrder.Status = "Delivered";
                    }
                    
                    LookupProductDetails(h.BranchId, returnOrder);
                    if(returnOrder.Items != null)
                    {
                        returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);
                    }
					
                    customerOrders.Add(returnOrder);
                }
                catch (Exception ex)
                {
                    _log.WriteErrorLog("Error proceesing order history for order: " + h.InvoiceNumber  + ".  " + ex.StackTrace);
                }
                
			}

			return customerOrders.ToList();
		}

		private void LookupProductDetails(string branchId, Order order)
		{
			if (order.Items == null) { return; }

			var products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber).ToList());

			var productDict = products.Products.ToDictionary(p => p.ItemNumber);

			Parallel.ForEach(order.Items, item =>
			{
				var prod = productDict.ContainsKey(item.ItemNumber) ? productDict[item.ItemNumber] : null;
				if (prod != null)
				{
					item.Name = prod.Name;
					item.Description = prod.Description;
					item.Pack = prod.Pack;
					item.Size = prod.Size;
					item.StorageTemp = prod.Nutritional.StorageTemp;
					item.Brand = prod.Brand;
					item.BrandExtendedDescription = prod.BrandExtendedDescription;
					item.ReplacedItem = prod.ReplacedItem;
					item.ReplacementItem = prod.ReplacementItem;
					item.NonStock = prod.NonStock;
					item.ChildNutrition = prod.ChildNutrition;
					item.CatchWeight = prod.CatchWeight;
					item.TempZone = prod.TempZone;
					item.ItemClass = prod.ItemClass;
					item.CategoryId = prod.CategoryId;
					item.CategoryName = prod.CategoryName;
					item.UPC = prod.UPC;
					item.VendorItemNumber = prod.VendorItemNumber;
					item.Cases = prod.Cases;
					item.Kosher = prod.Kosher;
					item.ManufacturerName = prod.ManufacturerName;
					item.ManufacturerNumber = prod.ManufacturerNumber;
					item.AverageWeight = prod.AverageWeight;
					item.Nutritional = new Nutritional()
					{
						CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
						GrossWeight = prod.Nutritional.GrossWeight,
						HandlingInstructions = prod.Nutritional.HandlingInstructions,
						Height = prod.Nutritional.Height,
						Length = prod.Nutritional.Length,
						Ingredients = prod.Nutritional.Ingredients,
						Width = prod.Nutritional.Width
					};
				}
				//if (price != null) {
				//    item.PackagePrice = price.PackagePrice.ToString();
				//    item.CasePrice = price.CasePrice.ToString();
				//}
			});

		}

		private List<Order> GetOrderHistoryOrders(UserSelectedContext customerInfo)
		{
			IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
																			   h.CustomerNumber.Equals(customerInfo.CustomerId),
																		  d => d.OrderDetails);
			return LookupControlNumberAndStatus(customerInfo,headers);
		}
		
		private List<Order> GetOrderHistoryHeadersForDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate)
		{
			IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
																			   h.CustomerNumber.Equals(customerInfo.CustomerId) && h.DeliveryDate >= startDate && h.DeliveryDate <= endDate, i => i.OrderDetails);
			return LookupControlNumberAndStatus(customerInfo, headers);
		}
		
		private List<Order> MergeOrderLists(List<Order> commerceServerOrders, List<Order> orderHistoryOrders)
		{
			System.Collections.Concurrent.BlockingCollection<Order> mergedOrdeList = new System.Collections.Concurrent.BlockingCollection<Order>();

			Parallel.ForEach(orderHistoryOrders, ohOrder =>
			{
				mergedOrdeList.Add(ohOrder);
			});

			Parallel.ForEach(commerceServerOrders, csOrder =>
			{
				if (mergedOrdeList.Where(o => o.InvoiceNumber.Equals(csOrder.InvoiceNumber)).Count() == 0)
				{ 
					mergedOrdeList.Add(csOrder);
				}
				//if (csOrder.InvoiceNumber.Equals("pending", StringComparison.InvariantCultureIgnoreCase)) {
				//    mergedOrdeList.Add(csOrder);
				//}
			});

			return mergedOrdeList.ToList();
		}

		#endregion
	}
}
