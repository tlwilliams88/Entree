using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Enumerations.Order;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceServer.Foundation;
using System.ServiceModel;
using System.Globalization;
using CommerceServer.Core;
using System.Data;
using System.Xml;

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
            IUnitOfWork unitOfWork, IEventLogRepository log, IGenericQueueRepository queue, IOrderConversionLogic conversionLogic, ICustomerRepository customerRepository) {
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
        private void Create(OrderHistoryFile currentFile) {
            // first attempt to find the order, look by confirmation number
            EF.OrderHistoryHeader header = null;

            if (!String.IsNullOrEmpty(currentFile.Header.ControlNumber) && !String.IsNullOrEmpty(currentFile.Header.OrderSystem.ToShortString())) {
                header = _headerRepo.ReadByConfirmationNumber(currentFile.Header.ControlNumber, currentFile.Header.OrderSystem.ToShortString()).FirstOrDefault();
            }

            // second attempt to find the order, look by invioce number
            if (header == null && !currentFile.Header.InvoiceNumber.Equals("Processing")) {
                header = _headerRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber).FirstOrDefault();
            }

            // last ditch effort is to create a new header
            if (header == null) {
                header = new EF.OrderHistoryHeader();
                header.OrderDetails = new List<EF.OrderHistoryDetail>();
            }

            currentFile.Header.MergeWithEntity(ref header);

            if (string.IsNullOrEmpty(header.OriginalControlNumber)) { header.OriginalControlNumber = currentFile.Header.ControlNumber; }

            foreach (OrderHistoryDetail currentDetail in currentFile.Details.ToList()) {

                EF.OrderHistoryDetail detail = null;

                if (header.OrderDetails != null && header.OrderDetails.Count > 0) {
                    detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();
                }

                if (detail == null) {
                    EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
                    tempDetail.BranchId = header.BranchId;
                    tempDetail.InvoiceNumber = header.InvoiceNumber;

                    header.OrderDetails.Add(tempDetail);
                } else {
                    currentDetail.MergeWithEntityFrameworkModel(ref detail);

                    detail.BranchId = header.BranchId;
                    detail.InvoiceNumber = header.InvoiceNumber;
                }
            }

            _headerRepo.CreateOrUpdate(header);
        }

        public PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging) {
            var headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                              h.CustomerNumber.Equals(customerInfo.CustomerId),
                                                                            d => d.OrderDetails);

            return LookupControlNumberAndStatus(customerInfo, headers).AsQueryable().GetPage(paging);
        }

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

                if (myOrder.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && myOrder.ControlNumber.Length > 0) {
                    po = _poRepo.ReadPurchaseOrderByTrackingNumber(myOrder.ControlNumber);
                    if (po != null) {
                        returnOrder.Status = po.Status;
                    }
                }
            }

            // Set the status to delivered if the Actual Delivery Time is populated
            if (returnOrder.ActualDeliveryTime.GetValueOrDefault() != DateTime.MinValue) {
                    returnOrder.Status = "Delivered";
            }

            if (myOrder != null) {
                try {
                    var invoice = _kpayInvoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(myOrder.BranchId), myOrder.CustomerNumber, myOrder.InvoiceNumber);
                    if (invoice != null) {
                        returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error looking up invoice when trying to get order:  " + ex.Message + ex.StackTrace);

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

        private List<Order> GetOrderHistoryHeadersForDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                               h.CustomerNumber.Equals(customerInfo.CustomerId) && h.DeliveryDate >= startDate && h.DeliveryDate <= endDate, i => i.OrderDetails);
            return LookupControlNumberAndStatus(customerInfo, headers);
        }

        private List<Order> GetOrderHistoryOrders(UserSelectedContext customerInfo) {
            IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                               h.CustomerNumber.Equals(customerInfo.CustomerId),
                                                                          d => d.OrderDetails);
            return LookupControlNumberAndStatus(customerInfo, headers);
        }

        public List<Order> GetOrders(Guid userId, UserSelectedContext customerInfo) {
            return GetOrderHistoryOrders(customerInfo).OrderByDescending(o => o.InvoiceNumber).ToList<Order>();
        }

        public List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            var customer = _customerRepository.GetCustomerByCustomerNumber(customerInfo.CustomerId, customerInfo.BranchId);

            var oh = GetOrderHistoryHeadersForDateRange(customerInfo, startDate, endDate);
            var cs = _poRepo.ReadPurchaseOrderHeadersInDateRange(customer.CustomerId, customerInfo.CustomerId, startDate, endDate);

            return MergeOrderLists(cs.Select(o => o.ToOrder()).ToList(), oh);
        }

        /// <summary>
        /// Gets just the primary order details needed without the excess of item details or invoice status
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<Order> GetShallowOrderDetailInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            List<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                               h.CustomerNumber.Equals(customerInfo.CustomerId) && h.DeliveryDate >= startDate && h.DeliveryDate <= endDate, i => i.OrderDetails).ToList();

            List<Order> orders = new List<Order>();

            foreach (EF.OrderHistoryHeader h in headers) {
                Order order = h.ToOrder();

                if (order.Items != null) {
                    order.OrderTotal = order.Items.Sum(i => i.LineTotal);
                }

                orders.Add( order );
            }

            // Leaving this code commented out in case we need further performance increases
            //Parallel.ForEach( headers, new ParallelOptions { MaxDegreeOfParallelism = 2 }, h => {
            //    Order order = h.ToOrder();

            //    if (order.Items != null) {
            //        order.OrderTotal = order.Items.AsParallel().WithDegreeOfParallelism(2).Sum(i => i.LineTotal);
            //    }

            //    orders.Add( order );
            //} );

            return orders;
        }

        /// <summary>
        /// Get a summary of order totals by month. Current month counts as 1.
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="numberOfMonths"></param>
        /// <returns></returns>
        public OrderTotalByMonth GetOrderTotalByMonth( UserSelectedContext customerInfo, int numberOfMonths ) {
            OrderTotalByMonth returnValue = new OrderTotalByMonth();

            // Need to get the last day of the current month
            DateTime end = new DateTime( DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth( DateTime.Today.Year, DateTime.Today.Month ) );
            // Need to get the first day from six months ago, create a new datetime object
            // Subtract the numberOfMonths but add 1 as the current month is intended as one of the results
            // Set the day to 1 for the start of the month
            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-numberOfMonths + 1).Month, 1);

            try {
                List<Order> orders = GetShallowOrderDetailInDateRange( customerInfo, start, end );

                // Iterate through the buckets and grab the sum for that month
                for (int i = 0; i <= numberOfMonths - 1; i++) {
                    DateTime currentMonth = start.AddMonths( i );

                    double bucketValue = (from o in orders
                                          where o.CreatedDate.Month == currentMonth.Month
                                          select o.OrderTotal).DefaultIfEmpty( 0 ).Sum();

                    returnValue.Totals.Add( bucketValue );
                }
                

                // Leaving this code commented out in case we need further performance increases
                //Parallel.For( 0, numberOfMonths - 1, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, i => {
                //    DateTime currentMonth = start.AddMonths( i );

                //    double bucketValue = (from o in orders
                //                          where o.CreatedDate.Month == currentMonth.Month
                //                          select o.OrderTotal).DefaultIfEmpty( 0 ).Sum();

                //    returnValue.Totals.Add( bucketValue );
                //} );
                
            } catch (Exception e) {
                _log.WriteErrorLog( String.Format( "Error getting order total by month for customer: {0}, branch: {1}", customerInfo.CustomerId, customerInfo.BranchId ), e );
                throw e;
            }

            return returnValue;
        }

        public void SaveOrder(OrderHistoryFile historyFile) {
            Create(historyFile);

            _unitOfWork.SaveChanges();
        }

        public void StopListening() {
            _keepListening = false;

            if (_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }

        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while (_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try {
                    var rawOrder = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, 
                                                                                      Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);

                    while (_keepListening && !string.IsNullOrEmpty(rawOrder)) {
                        OrderHistoryFile historyFile = new OrderHistoryFile();

                        historyFile = JsonConvert.DeserializeObject<OrderHistoryFile>(rawOrder);

                        _log.WriteInformationLog(string.Format("Consuming order update from queue for message ({0})", historyFile.MessageId));

                        Create(historyFile);
                        _conversionLogic.SaveOrderHistoryAsConfirmation(historyFile);

                        _unitOfWork.SaveChangesAndClearContext();

                        rawOrder = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, 
                                                                                     Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);
                    }

                } catch (Exception ex) {
                    KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, subject: "Exception processing Order History in Queue Service");

                    _log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }

        }

        //private List<Order> LookupControlNumberAndStatus(UserSelectedContext userContext, IEnumerable<EF.OrderHistoryHeader> headers) {
        //    var customerOrders = new BlockingCollection<Order>();
        //    foreach (var h in headers) {
        //        try {
        //            Order returnOrder = null;

        //            returnOrder = h.ToOrder();

        //            if (h.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && h.ControlNumber.Length > 0) {
        //                var po = _poRepo.ReadPurchaseOrderByTrackingNumber(h.ControlNumber);
        //                if (po != null) {
        //                    returnOrder.Status = po.Status;
        //                    returnOrder.OrderNumber = h.ControlNumber;
        //                    returnOrder.IsChangeOrderAllowed = (po.Properties["MasterNumber"] != null && (po.Status.StartsWith("Confirmed")));
        //                }

        //            }

        //            var invoice = _kpayInvoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, returnOrder.InvoiceNumber);
        //            if (invoice != null) {
        //                returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
        //            }

        //            if (returnOrder.ActualDeliveryTime != null) {
        //                returnOrder.Status = "Delivered";
        //            }

        //            //LookupProductDetails(h.BranchId, returnOrder);
        //            if (returnOrder.Items != null) {
        //                returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);
        //            }

        //            customerOrders.Add(returnOrder);
        //        } catch (Exception ex) {
        //            _log.WriteErrorLog("Error proceesing order history for order: " + h.InvoiceNumber + ".  " + ex.StackTrace);
        //        }

        //    }

        //    return customerOrders.ToList();
        //}

        private List<Order> LookupControlNumberAndStatus( UserSelectedContext userContext, IEnumerable<EF.OrderHistoryHeader> headers ) {
            var customerOrders = new BlockingCollection<Order>();

            // Get the customer GUID to retrieve all purchase orders from commerce server
            var customerInfo = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
            var POs = _poRepo.ReadPurchaseOrderHeadersByCustomerId(customerInfo.CustomerId);

            foreach (var h in headers) {
                try {
                    Order returnOrder = null;

                    returnOrder = h.ToOrder();

                    LookupProductDetails(h.BranchId, returnOrder);

                    if (h.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && h.ControlNumber.Length > 0) {
                        // Check if the purchase order exists and grab it for additional information if it does
                        if (POs != null) {
                            PurchaseOrder currentPo = POs.Where( x => x.Properties["OrderNumber"].Equals( h.ControlNumber ) ).FirstOrDefault<PurchaseOrder>();

                            if (currentPo != null) {
                                returnOrder.Status = currentPo.Status;
                                returnOrder.OrderNumber = h.ControlNumber;
                                returnOrder.IsChangeOrderAllowed = (currentPo.Properties["MasterNumber"] != null && (currentPo.Status.StartsWith( "Confirmed" )));
                            }
                        }
                    }

                    var invoice = _kpayInvoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, returnOrder.InvoiceNumber);
                    if (invoice != null) {
                        returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
                    }

                    if (returnOrder.ActualDeliveryTime != null) {
                        returnOrder.Status = "Delivered";
                    }

                    //LookupProductDetails(h.BranchId, returnOrder);
                    if (returnOrder.Items != null) {
                        returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);
                    }

                    customerOrders.Add(returnOrder);
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error proceesing order history for order: " + h.InvoiceNumber + ".  " + ex.StackTrace);
                }

            }

            return customerOrders.ToList();
        }

        private void LookupProductDetails(string branchId, Order order) {
            if (order.Items == null) { return; }

            var products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber).ToList());

            var productDict = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(order.Items, item => {
                var prod = productDict.ContainsKey(item.ItemNumber) ? productDict[item.ItemNumber] : null;
                if (prod != null) {
                    item.IsValid = true;
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
            BlockingCollection<Order> mergedOrdeList = new BlockingCollection<Order>();

            Parallel.ForEach(orderHistoryOrders, ohOrder => {
                mergedOrdeList.Add(ohOrder);
            });

            Parallel.ForEach(commerceServerOrders, csOrder => {
                if (mergedOrdeList.Where(o => o.InvoiceNumber.Equals(csOrder.InvoiceNumber)).Count() == 0) {
                    mergedOrdeList.Add(csOrder);
                }
            });

            return mergedOrdeList.ToList();
        }

        public string CheckForLostOrders(out string sBody)
        {
            StringBuilder sbMsgSubject = new StringBuilder();
            StringBuilder sbMsgBody = new StringBuilder();

            CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, "Pending");
            CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, "Submitted");

            sBody = sbMsgBody.ToString();
            return sbMsgSubject.ToString();
        }

        private void CheckForLostOrdersByStatus(StringBuilder sbMsgSubject, StringBuilder sbMsgBody, string qStatus)
        {
            List<System.Xml.XmlElement> Pos;
            GetPurchaseOrdersByStatus(qStatus, out Pos);
            StringBuilder sbAppendSubject;
            StringBuilder sbAppendBody;
            BuildAlertStringsForLostPurchaseOrders(out sbAppendSubject, out sbAppendBody, Pos, qStatus);
            if (sbAppendSubject.Length > 0)
            {
                sbMsgSubject.Append(sbAppendSubject.ToString());
            }
            if (sbAppendBody.Length > 0)
            {
                sbMsgBody.Append(sbAppendBody.ToString());
            }
        }

        private void GetPurchaseOrdersByStatus(string queryStatus, out List<System.Xml.XmlElement> Pos)
        {
            var manager = CommerceServerCore.GetPoManager();
            System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            // set what to search
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            // set what field/value to search for
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", queryStatus);
            // set what fields to return
            DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

            int c = results.Tables.Count;

            Pos = new List<System.Xml.XmlElement>();
            List<Guid> poIds = new List<Guid>();
            foreach (DataRow row in results.Tables[0].Rows)
            {
                poIds.Add(new Guid(row["OrderGroupId"].ToString()));
            }
            // Get the XML representation of the purchase orders.
            if (poIds.Count > 0)
            {
                foreach (var poid in poIds)
                {
                    List<Guid> single = new List<Guid>();
                    single.Add(poid);
                    var xml = manager.GetPurchaseOrdersAsXml(single.ToArray());
                    Pos.Add(xml);
                }
            }
        }

        private void BuildAlertStringsForLostPurchaseOrders(out StringBuilder sbSubject, out StringBuilder sbBody, List<System.Xml.XmlElement> Pos, string qStatus)
        {
            sbSubject = new StringBuilder();
            sbBody = new StringBuilder();
            if ((Pos != null) && (Pos.Count > 0))
            {
                foreach (var xml in Pos)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml.InnerXml);
                    //var nRoot = doc.GetElementsByTagName("PurchaseOrder")[0];
                    DateTime lastModified;
                    DateTime.TryParse(doc.DocumentElement.Attributes.GetNamedItem("LastModified").Value, out lastModified);
                    // only if they've been created more than 10 minutes ago in the query status
                    if (lastModified < DateTime.Now.AddMinutes(-10))
                    {
                        sbSubject.Clear();
                        sbSubject.Append("PO in a " + qStatus + " status for more than 10 minutes.");
                        sbBody.Append("* PO");
                        sbBody.Append(" for ");
                        XmlNodeList childs = doc.DocumentElement.GetElementsByTagName("WeaklyTypedProperties");
                        XmlDocument elem = new XmlDocument();
                        elem.LoadXml("<Info>" + childs[childs.Count - 1].InnerXml + "</Info>");
                        {
                            foreach (XmlElement child in elem.DocumentElement.GetElementsByTagName("WeaklyTypedProperty"))
                            {
                                if (child.Attributes["Name"].Value.Equals("customerid", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sbBody.Append(child.Attributes["Value"].Value);
                                }
                            }
                        }
                        //sbBody.Append((doc.DocumentElement.GetElementsByTagName("WeaklyTypedProperties")[0]).g);
                        sbBody.Append("-");
                        {
                            foreach (XmlElement child in elem.DocumentElement.GetElementsByTagName("WeaklyTypedProperty"))
                            {
                                if (child.Attributes["Name"].Value.Equals("branchid", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sbBody.Append(child.Attributes["Value"].Value.ToUpper());
                                }
                            }
                        }
                        sbBody.Append(" with cart ");
                        {
                            sbBody.Append(doc.DocumentElement.Attributes["Name"].Value);
                        }
                        sbBody.Append(" and tracking ");
                        {
                            sbBody.Append(doc.DocumentElement.Attributes["TrackingNumber"].Value);
                        }
                        sbBody.Append(" last modified " + lastModified.ToShortDateString());
                        sbBody.Append(" at " + lastModified.ToShortTimeString());
                        sbBody.Append(" in status " + qStatus);
                        sbBody.Append(".\n");
                    }
                }
            }
        }
        #endregion
    }
}
// An example of each purchase order xml
//<PurchaseOrder BillingCurrency="" Status="Submitted" SoldToId="39f1ac2d-494a-4a65-bf83-68c020e3f815" TaxTotal="0.0000" 
//    LastModified="2015-11-17T13:01:04.887-06:00" Created="2015-11-17T13:01:04.887-06:00" StatusCode="PurchaseOrder" SoldToName="" 
//    TrackingNumber="0001083" SubTotal="130.8700" IsDirty="false" LineItemCount="4" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" 
//    HandlingTotal="0.0000" BasketId="655811bf-b90d-4804-894b-5aebf6425cc1" ShippingTotal="0.0000" ModifiedBy="" Total="130.8700" 
//    Name="sfdf_726971_NewCart0">
//    <OrderForms>
//        <OrderForm Status="InProcess" TaxTotal="0.0000" LastModified="2015-11-17T13:01:04.82-06:00" Created="2015-11-17T13:00:50.793-06:00" 
//            PromoUserIdentity="" SubTotal="130.8700" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" HandlingTotal="0.0000" 
//            ShippingTotal="0.0000" ModifiedBy="" BillingAddressId="" Total="130.8700" OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" 
//            Name="Default">
//            <PromoCodeRecords />
//            <Shipments />
//            <PromoCodes />
//            <Payments />
//            <LineItems>
//                <LineItem ShippingMethodName="" ProductId="630420" PlacedPrice="50.2000" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="50.2000" 
//                    Quantity="1.0000" DisplayName="Drink Mix Fruit Punch" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.847-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="72c4ea80-dd99-482a-970a-f1079520379e">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Notes" Value="test note" Type="String" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="1" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="102968" PlacedPrice="19.8500" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="19.8500" 
//                    Quantity="1.0000" DisplayName="Juice Cranberry Cocktail 10%" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.86-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="744a1c39-bf4b-4a6d-befa-14f1246b1dcc">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="2" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="630613" PlacedPrice="53.1500" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" 
//                    AllowBackordersAndPreorders="true" LastModified="2015-11-17T13:01:04.82-06:00" 
//                    OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="53.1500" Quantity="1.0000" 
//                    DisplayName="Gatorade Fierce Grape" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.863-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="0d2ccad1-ef11-400f-b699-a89f204b4ac5"><ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="3" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="098073" PlacedPrice="7.6700" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="7.6700" 
//                    Quantity="1.0000" DisplayName="Carrot Match Stick" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.863-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="f12f38bb-1249-4c01-9062-c691b7cb4f34">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="true" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="4" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 2" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//            </LineItems>
//            <WeaklyTypedProperties>
//                <WeaklyTypedProperty Name="BranchId" Value="fdf" Type="String" />
//                <WeaklyTypedProperty Name="Shared" Value="true" Type="Boolean" />
//                <WeaklyTypedProperty Name="ListType" Value="3" Type="Int32" />
//                <WeaklyTypedProperty Name="BasketLevelDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="DiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="CustomerId" Value="726971" Type="String" />
//                <WeaklyTypedProperty Name="ShippingDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="TempSubTotal" Value="130.87" Type="Decimal" />
//                <WeaklyTypedProperty Name="RequestedShipDate" Value="2015-11-18T00:00:00-06:00" Type="DateTime" />
//                <WeaklyTypedProperty Name="LineItemDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="DisplayName" Value="New Cart 0" Type="String" />
//                <WeaklyTypedProperty Name="BasketType" Value="0" Type="Int32" />
//            </WeaklyTypedProperties>
//        </OrderForm>
//    </OrderForms>
//    <Addresses />
//    <WeaklyTypedProperties>
//        <WeaklyTypedProperty Name="OriginalOrderNumber" Value="0001083" Type="String" />
//        <WeaklyTypedProperty Name="DiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="ListType" Value="3" Type="Int32" />
//        <WeaklyTypedProperty Name="BasketLevelDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="BranchId" Value="fdf" Type="String" />
//        <WeaklyTypedProperty Name="RequestedShipDate" Value="2015-11-18T00:00:00-06:00" Type="DateTime" />
//        <WeaklyTypedProperty Name="ShippingDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="LineItemDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="TempSubTotal" Value="130.87" Type="Decimal" />
//        <WeaklyTypedProperty Name="DisplayName" Value="New Cart 0" Type="String" />
//        <WeaklyTypedProperty Name="Shared" Value="true" Type="Boolean" />
//        <WeaklyTypedProperty Name="CustomerId" Value="726971" Type="String" />
//        <WeaklyTypedProperty Name="BasketType" Value="0" Type="Int32" />
//    </WeaklyTypedProperties>
//</PurchaseOrder>
