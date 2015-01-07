using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryLogicImpl : IOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;
        private const int THREAD_SLEEP_DURATION = 2000;

        private readonly ICatalogLogic _catalogLogic;
        private readonly IConfirmationLogic _confirmationLogic;
        private readonly IEventLogRepository _log;
        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IOrderHistoryQueueRepository _queue;
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly ISocketListenerRepository _socket;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ICustomerRepository _customerRepository;
        //private readonly IUnitOfWork _unitOfWorkOriginal;

        private bool _keepListening;
        private Task _queueTask;
        #endregion

        #region ctor
        public OrderHistoryLogicImpl(IEventLogRepository logRepo, IOrderHistoryHeaderRepsitory headerRepo, IOrderHistoryDetailRepository detailRepo, IOrderHistoryQueueRepository queueRepo,
                                     IUnitOfWork unitOfWork, IConfirmationLogic confLogic, ISocketListenerRepository socket, IPurchaseOrderRepository poRepo, ICatalogLogic catalogLogic,
                                     IUserProfileRepository userProfileRepository, ICustomerRepository customerRepository) {
            _catalogLogic = catalogLogic;
            _confirmationLogic = confLogic;
            _log = logRepo;
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
            _poRepo = poRepo;
            _queue = queueRepo;
            _socket = socket;
            _unitOfWork = unitOfWork;
            _userProfileRepository = userProfileRepository;
            _customerRepository = customerRepository;
            
            _keepListening = true;

            _socket.FileReceived            += SocketFileReceived;
            _socket.ClosedPort              += SocketPortClosed;
            _socket.OpeningPort             += SocketOpeningPort;
            _socket.WaitingConnection       += SocketWaitingConnection;
            _socket.BeginningFileReceipt    += SocketBeginningFileReceipt;
            _socket.ErrorEncountered        += SocketExceptionEncountered;
        }
        #endregion

        #region events
        public void SocketFileReceived(object sender, ReceivedFileEventArgs e) {
            StringBuilder logMsg = new StringBuilder();
            logMsg.AppendLine("Order Update File Received. See below for more details.");
            logMsg.AppendLine();
            logMsg.AppendLine(e.FileData);

            _log.WriteInformationLog(logMsg.ToString());
            string[] lines = e.FileData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            OrderHistoryFileReturn parsedFiles = ParseFile(lines);

            foreach (OrderHistoryFile parsedFile in parsedFiles.Files) {
                StringWriter xmlWriter = new StringWriter();
                XmlSerializer xs = new XmlSerializer(parsedFile.GetType());

                xs.Serialize(xmlWriter, parsedFile);

                _queue.PublishToQueue(xmlWriter.ToString());
            }
        }

        public void SocketPortClosed(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port closed");
        }

        public void SocketOpeningPort(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port opening");
        }

        public void SocketWaitingConnection(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port connecting");
        }

        public void SocketBeginningFileReceipt(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener beginning file receipt");
        }

        public void SocketExceptionEncountered(object sender, ExceptionEventArgs e) {
            _log.WriteErrorLog(e.Exception.Message);
        }
        #endregion

        #region methods
        private void Create(OrderHistoryFile currentFile) {
            EF.OrderHistoryHeader header = _headerRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber).FirstOrDefault();
            if (header == null) {
                header = new EF.OrderHistoryHeader();
                header.OrderDetails = new List<EF.OrderHistoryDetail>();
            }

            currentFile.Header.MergeWithEntity(ref header);

            foreach (OrderHistoryDetail currentDetail in currentFile.Details) {

                EF.OrderHistoryDetail detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();

                if (detail == null) {
                    EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
                    tempDetail.BranchId = header.BranchId;
                    tempDetail.InvoiceNumber = header.InvoiceNumber;

                    header.OrderDetails.Add(currentDetail.ToEntityFrameworkModel());
                } else {
                    currentDetail.MergeWithEntityFrameworkModel(ref detail);

                    detail.BranchId = header.BranchId;
                    detail.InvoiceNumber = header.InvoiceNumber;
                }
            }

            _headerRepo.CreateOrUpdate(header);

            //_unitOfWork.SaveChanges();
        }

        public void ListenForMainFrameCalls() {
            _socket.Listen(Configuration.MainframOrderHistoryListeningPort);
        }

        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while (_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                int loopCnt = 0;
                //IUnitOfWork uow = _unitOfWork.GetUniqueUnitOfWork();

                try {
                    StringBuilder rawOrder = new StringBuilder(_queue.ConsumeFromQueue());

                    while (rawOrder.Length > 0) {
                        OrderHistoryFile historyFile = new OrderHistoryFile();

                        _log.WriteInformationLog(string.Format("Consuming order update from queue for message ({0})", historyFile.MessageId));

                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(historyFile.GetType());
                        System.IO.StringReader xmlData = new System.IO.StringReader(rawOrder.ToString());

                        historyFile = (OrderHistoryFile)xs.Deserialize(xmlData);

                        Create(historyFile);
                        ProcessAsConfirmation(historyFile);

                        rawOrder = new StringBuilder(_queue.ConsumeFromQueue());

                        if (loopCnt++ == 100) {
                            _unitOfWork.SaveChangesAndClearContext();
                            //uow.SaveChangesAndClearContext();
                            //uow = _unitOfWork.GetUniqueUnitOfWork();
                            loopCnt = 0;
                        }
                    }

                    if (loopCnt > 0) {
                        _unitOfWork.SaveChangesAndClearContext();
                        //uow.SaveChangesAndClearContext();
                        //uow = _unitOfWork.GetUniqueUnitOfWork();

                        loopCnt = 0;
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }

        }

        private List<Order> GetCommerceServerOrders(Guid userId, UserSelectedContext customerInfo) {

            // get all users to then read all orders from CS
            List<PurchaseOrder> allOrders = new List<PurchaseOrder>();
            var sharedUsers = _userProfileRepository.GetUsersForCustomerOrAccount(_customerRepository.GetCustomerByCustomerNumber(customerInfo.CustomerId).CustomerId).ToList();

            foreach (var user in sharedUsers)
            {
                allOrders.AddRange(_poRepo.ReadPurchaseOrders(user.UserId, customerInfo.CustomerId));
            }
            return allOrders.Select(o => o.ToOrder()).ToList();
        }

        public List<Order> GetOrders(Guid userId, UserSelectedContext customerInfo) {
            List<Order> customerOrders = MergeOrderLists(GetCommerceServerOrders(userId, customerInfo) ,
                                                         GetOrderHistoryOrders(customerInfo));

            return customerOrders.OrderByDescending(o => o.InvoiceNumber).ToList<Order>();
        }

        public List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            var oh = GetOrderHistoryHeadersForDateRange(customerInfo, startDate, endDate);
            var cs = _poRepo.ReadPurchaseOrderHeadersInDateRange(userId, customerInfo.CustomerId, startDate, endDate);

            return MergeOrderLists(cs.Select(o => o.ToOrder()).ToList(), oh);
        }

        private List<Order> GetOrderHistoryHeadersForDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                               h.CustomerNumber.Equals(customerInfo.CustomerId) && h.CreatedUtc >= startDate && h.CreatedUtc <= endDate, i => i.OrderDetails);
            return LookupControlNumberAndStatus(headers);
        }

        private List<Order> GetOrderHistoryOrders(UserSelectedContext customerInfo) {
            IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                               h.CustomerNumber.Equals(customerInfo.CustomerId),
                                                                          d => d.OrderDetails);
            return LookupControlNumberAndStatus(headers);
        }

		private List<Order> LookupControlNumberAndStatus(IEnumerable<EF.OrderHistoryHeader> headers)
		{
			var customerOrders = new BlockingCollection<Order>();
			Parallel.ForEach(headers, h =>
			{
				Order currentOrder = h.ToOrderHeaderOnly();

				if (h.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && h.ControlNumber.Length > 0)
				{
					PurchaseOrder po = _poRepo.ReadPurchaseOrderByTrackingNumber(h.ControlNumber);

					if (po != null)
					{
						currentOrder.OrderNumber = h.ControlNumber;
						currentOrder.Status = po.Status;
					}
				}

				customerOrders.Add(currentOrder);
			});

			return customerOrders.ToList();
		}

        private void LookupProductDetails(string branchId, Order order) {
            if (order.Items == null) { return; }

            var products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber).ToList());

			var productDict = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(order.Items, item => {
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
                    item.Nutritional = new Nutritional() {
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

        private List<Order> MergeOrderLists(List<Order> commerceServerOrders, List<Order> orderHistoryOrders) {
            System.Collections.Concurrent.BlockingCollection<Order> mergedOrdeList = new System.Collections.Concurrent.BlockingCollection<Order>();

            Parallel.ForEach(orderHistoryOrders, ohOrder => {
                mergedOrdeList.Add(ohOrder);
            });

            Parallel.ForEach(commerceServerOrders, csOrder => {
                if (mergedOrdeList.Where(o => o.InvoiceNumber.Equals(csOrder.InvoiceNumber)).Count() == 0) {
                    mergedOrdeList.Add(csOrder);
                }
                //if (csOrder.InvoiceNumber.Equals("pending", StringComparison.InvariantCultureIgnoreCase)) {
                //    mergedOrdeList.Add(csOrder);
                //}
            });

            return mergedOrdeList.ToList();
        }

        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private OrderHistoryFileReturn ParseFile(string[] data) {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            OrderHistoryFile currentFile = null;

            for (int i = 0; i < data.Length; i++) {
                if (data[i].Contains("END###")) { break; }

                switch (data[i].Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                    case "H":
                        if (currentFile != null) {
                            currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                                where detail.ItemStatus != string.Empty
                                                                select true).FirstOrDefault();
                            currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                                where detail.FutureItem == true
                                                                select true).FirstOrDefault();
                            retVal.Files.Add(currentFile);
                        }

                        currentFile = new OrderHistoryFile();

                        currentFile.Header.Parse(data[i]);
                        break;
                    case "D":
                        if (currentFile != null) {
                            OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                            orderDetail.Parse(data[i]);

                            currentFile.Details.Add(orderDetail);
                        }
                        break;
                    default:
                        break;
                }

            } // end of for loop

            if (currentFile != null) {
                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                    where detail.ItemStatus != string.Empty
                                                    select true).FirstOrDefault();
                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                    where detail.FutureItem == true
                                                    select true).FirstOrDefault();
                retVal.Files.Add(currentFile);
            }

            return retVal;
        }

        public OrderHistoryFileReturn ParseMainframeFile(string filePath)
        {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            using (System.IO.TextReader txtFile = System.IO.File.OpenText(filePath)) {
                OrderHistoryFile currentFile = null;

                while (txtFile.Peek() != -1) {
                    string data = txtFile.ReadLine();

                    switch (data.Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                        case "H":
                            if (currentFile != null) {
                                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.ItemStatus != string.Empty
                                                                  select true).FirstOrDefault();
                                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.FutureItem == true
                                                                  select true).FirstOrDefault();
                                retVal.Files.Add(currentFile); 
                            }
                            
                            currentFile = new OrderHistoryFile();

                            currentFile.Header.Parse(data);
                            break;
                        case "D":
                            if (currentFile != null) {
                                OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                                orderDetail.Parse(data);

                                currentFile.Details.Add(orderDetail);
                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if (currentFile != null) {
                    currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.ItemStatus != string.Empty
                                                      select true).FirstOrDefault();
                    currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.FutureItem == true
                                                      select true).FirstOrDefault();
                    retVal.Files.Add(currentFile);
                }
            }

            return retVal;
        }

        private void ProcessAsConfirmation(OrderHistoryFile historyFile) {
            // if entree order, convert to confirmation and process the confirmation
            if (historyFile.Header.OrderSystem != OrderSource.Entree) { return; }

            ConfirmationFile confirmation = new ConfirmationFile();

            foreach (OrderHistoryDetail historyDetail in historyFile.Details) {
                ConfirmationDetail detail = new ConfirmationDetail() {
                    RecordNumber = historyDetail.LineNumber.ToString(),
                    ItemNumber = historyDetail.ItemNumber,
                    QuantityOrdered = historyDetail.OrderQuantity,
                    BrokenCase = (historyDetail.UnitOfMeasure == UnitOfMeasure.Package ? "Y" : "N"),
                    QuantityShipped = historyDetail.ShippedQuantity,
                    ShipWeight = historyDetail.TotalShippedWeight,
                    SalesGross = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    SalesNet = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    PriceNet = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    PriceGross = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    SplitPriceNet = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? 0.0 : historyDetail.SellPrice),
                    SplitPriceGross = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? 0.0 : historyDetail.SellPrice),
                    ReasonNotShipped = historyDetail.ItemStatus
                    //CaseCube
                    //CaseWeight
                };

                detail.ConfirmationMessage = detail.DisplayStatus();
                confirmation.Detail.Add(detail);
            }

            confirmation.Header.Branch = historyFile.Header.BranchId;
            confirmation.Header.ConfirmationNumber = historyFile.Header.ControlNumber;
            confirmation.Header.CustomerNumber = historyFile.Header.CustomerNumber;
            confirmation.Header.InvoiceNumber = historyFile.Header.InvoiceNumber;
            confirmation.Header.ConfirmationDate = DateTime.Now;
            confirmation.Header.ShipDate = historyFile.Header.DeliveryDate;
            confirmation.Header.RemoteOrderNumber = historyFile.Header.ControlNumber;
            confirmation.Header.RouteNumber = historyFile.Header.RouteNumber;
            confirmation.Header.StopNumber = historyFile.Header.StopNumber;
            confirmation.Header.TotalQuantityOrdered = confirmation.Detail.Sum(d => d.QuantityOrdered);
            confirmation.Header.TotalQuantityShipped = confirmation.Detail.Sum(d => d.QuantityShipped);
            //confirmation.Header.TotalInvoice
            confirmation.Header.ConfirmationStatus = historyFile.Header.OrderStatus;
            confirmation.Header.ConfirmationMessage = confirmation.Header.GetDisplayStatus();
            //confirmation.Header.SpecialInstructions
            //confirmation.Header.SpecialInstructionsExtended
            //confirmation.Header.TotalCube
            //confirmation.Header.TotalWeight

            _confirmationLogic.ProcessIncomingConfirmation(confirmation);
        }

        public Order ReadOrder(string branchId, string orderNumber) {
            EF.OrderHistoryHeader myOrder = _headerRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                  h.InvoiceNumber.Equals(orderNumber),
                                                             d => d.OrderDetails).FirstOrDefault();
            Order returnOrder = null;

            if (myOrder == null) {
                PurchaseOrder po = _poRepo.ReadPurchaseOrderByTrackingNumber(orderNumber);
                returnOrder = po.ToOrder();
            } else {
                 returnOrder = myOrder.ToOrder();
            }

            LookupProductDetails(branchId, returnOrder);

            
            return returnOrder;
        }

        public void Save(OrderHistoryFile currentFile) {
            Create(currentFile);

            try {
                _unitOfWork.SaveChanges();
            } catch (Exception ex) {
                throw;
            }
        }

        public void StopListening() {
            _keepListening = false;

            if (_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }
        #endregion
    }
}
