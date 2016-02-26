using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
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
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using CommerceServer.Foundation;
using CommerceServer.Core;

using Newtonsoft.Json;
using System;
using System.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KeithLink.Svc.Impl.Logic.InternalSvc {
    public class InternalOrderHistoryLogic : IInternalOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;
        private const int THREAD_SLEEP_DURATION = 2000;



        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IOrderHistoryDetailRepository _detailRepo;
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
            IPurchaseOrderRepository poRepo, IKPayInvoiceRepository kpayInvoiceRepository, ICatalogLogic catalogLogic, IOrderHistoryDetailRepository detailRepo,
            IUnitOfWork unitOfWork, IEventLogRepository log, IGenericQueueRepository queue, IOrderConversionLogic conversionLogic, ICustomerRepository customerRepository) {
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
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
        private void Create(OrderHistoryFile currentFile, bool isSpecialOrder) {
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

            // set isSpecialOrder if that is true; but don't set otherwise (used from two places)
            if (isSpecialOrder) {
                header.IsSpecialOrder = true;
            }

            if (string.IsNullOrEmpty(header.OriginalControlNumber)) { header.OriginalControlNumber = currentFile.Header.ControlNumber; }

            bool hasSpecialItems = false;

            foreach (OrderHistoryDetail currentDetail in currentFile.Details.ToList()) {
                if (string.IsNullOrWhiteSpace(currentDetail.SpecialOrderHeaderId)) {
                    hasSpecialItems = true;
                }

                EF.OrderHistoryDetail detail = null;

                if (header.OrderDetails != null && header.OrderDetails.Count > 0) {
                    detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();
                }

                if (detail == null) {
                    EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
                    tempDetail.BranchId = header.BranchId;
                    tempDetail.InvoiceNumber = header.InvoiceNumber;
                    tempDetail.OrderHistoryHeader = header;

                    if (isSpecialOrder) {
                        tempDetail.ItemStatus = KeithLink.Svc.Core.Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
                    }

                    header.OrderDetails.Add(tempDetail);
                } else {
                    currentDetail.MergeWithEntityFrameworkModel(ref detail);

                    detail.BranchId = header.BranchId;
                    detail.InvoiceNumber = header.InvoiceNumber;
                    if (isSpecialOrder) {
                        detail.ItemStatus = KeithLink.Svc.Core.Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
                    }
                }
            }

            _headerRepo.CreateOrUpdate(header);

            if (hasSpecialItems) {
                RemoveSpecialOrderItemsFromHistory(header);
            }
        }

        public PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging) {
            List<EF.OrderHistoryHeader> headers = _headerRepo.Read( h => h.BranchId.Equals( customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase )
                                                                    && h.CustomerNumber.Equals( customerInfo.CustomerId ), 
                                                                    d => d.OrderDetails ).ToList();

            return LookupControlNumberAndStatus( customerInfo, headers ).AsQueryable().GetPage( paging );
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            EF.OrderHistoryHeader myOrder = _headerRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                  (h.InvoiceNumber.Equals(invoiceNumber) || h.ControlNumber.Equals(invoiceNumber)),
                                                             d => d.OrderDetails).FirstOrDefault();
            PurchaseOrder po = null;

            Order returnOrder = null;

            if (myOrder == null) {
                po = _poRepo.ReadPurchaseOrderByTrackingNumber(invoiceNumber);
                //_log.WriteInformationLog("InternalOrderHistoryLogic.GetOrder() invoiceNumber=" + invoiceNumber);
                returnOrder = po.ToOrder();
                if (po != null)
                {
                    PullCatalogFromPurchaseOrderItemsToOrder(po, returnOrder);
                }
            }
            else
            {
                returnOrder = myOrder.ToOrder();

                if (myOrder.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && myOrder.ControlNumber.Length > 0) {
                    po = _poRepo.ReadPurchaseOrderByTrackingNumber(myOrder.ControlNumber);
                    if (po != null) {
                        returnOrder.Status = po.Status;
                        returnOrder.CommerceId = Guid.Parse(po.Id);
                        PullCatalogFromPurchaseOrderItemsToOrder(po, returnOrder);

                        if (po.Status == "Confirmed with un-submitted changes") {
                            returnOrder = po.ToOrder();
                        }


                        // needed to reconnect parent orders to special orders
                        if (myOrder.RelatedControlNumber == null) {		
                            FindOrdersRelatedToPurchaseOrder(po, returnOrder, myOrder, null);		
                        } else {		
                            returnOrder.RelatedOrderNumbers = myOrder.RelatedControlNumber;		
                        }
                    }
                }
            }

            // Set the status to delivered if the Actual Delivery Time is populated
            //if (returnOrder.ActualDeliveryTime.GetValueOrDefault() != DateTime.MinValue) {
            if (!string.IsNullOrEmpty(returnOrder.ActualDeliveryTime)) {
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

            if ( returnOrder.CatalogId == null ) {
                returnOrder.CatalogId = branchId;
            }

            LookupProductDetails(returnOrder.CatalogId, returnOrder);

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

        /// <summary>
        /// Pulls orderhistory orders for that same customer within a few minutes (either before or after) and calls those related (stores the order number and invoice number) in the order
        /// </summary>
        /// <param name="po">The purchase order that the items are stored in</param>
        /// <param name="returnOrder">The order that the items are being set on</param>
        /// <param name="headers">The reference to order history headers if we have it</param>
        /// <returns></returns>
        private void FindOrdersRelatedToPurchaseOrder(PurchaseOrder po, Order returnOrder, EF.OrderHistoryHeader thisOrder, List<EF.OrderHistoryHeader> headers)
        {
            string customerNumber = po.Properties["CustomerId"].ToString();
            string branchId = po.Properties["BranchId"].ToString();
            string orderNumber = po.Properties["OrderNumber"].ToString();
            //string controlNumber = po.Properties["OrderNumber"].ToString();
            
            if (headers == null)
                headers = _headerRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                 h.CustomerNumber.Equals(customerNumber) &&
                                                 h.RelatedControlNumber == orderNumber).ToList();

            StringBuilder sbRelatedOrders = new StringBuilder();
            StringBuilder sbRelatedInvoices = new StringBuilder();
            
            foreach (var item in headers)
            {
                if (sbRelatedOrders.Length > 0) sbRelatedOrders.Append(",");
                if (sbRelatedInvoices.Length > 0) sbRelatedInvoices.Append(",");

                sbRelatedOrders.Append(item.ControlNumber);
                sbRelatedInvoices.Append(item.InvoiceNumber);
            }

            returnOrder.RelatedOrderNumbers = sbRelatedOrders.ToString();
            returnOrder.RelatedInvoiceNumbers = sbRelatedInvoices.ToString();
        }

        private List<Order> GetOrderHistoryHeadersForDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            List<EF.OrderHistoryHeader> headers = _headerRepo.Read( h => h.BranchId.Equals( customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase )
                                                                               && h.CustomerNumber.Equals( customerInfo.CustomerId ) 
                                                                               && h.DeliveryDate.ToDateTime() >= startDate 
                                                                               && h.DeliveryDate.ToDateTime() <= endDate, i => i.OrderDetails ).ToList();
            return LookupControlNumberAndStatus(customerInfo, headers);
        }

        private List<Order> GetOrderHistoryOrders(UserSelectedContext customerInfo) {
            List<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)
                                                                          && h.CustomerNumber.Equals( customerInfo.CustomerId ),
                                                                          d => d.OrderDetails).ToList();
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
            int numberOfDays = (endDate - startDate).Days + 1;
            var dateRange = Enumerable.Range(0, numberOfDays).Select(d => startDate.AddDays(d).ToLongDateFormat());

            List<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) 
                                                                                                       && h.CustomerNumber.Equals(customerInfo.CustomerId) 
                                                                                                       && dateRange.Contains(h.DeliveryDate),
                                                                                                    i => i.OrderDetails)
                                                                                           .ToList();
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
            // Need to get the first day from six months ago, create a new datetime object at the first of the current month
            // Subtract the numberOfMonths but add 1 as the current month is intended as one of the results
            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            start = start.AddMonths( -numberOfMonths + 1 );

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
        
        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while (_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try {
                    var rawOrder = ReadOrderFromQueue();

                    while (_keepListening && !string.IsNullOrEmpty(rawOrder)) {
                        OrderHistoryFile historyFile = new OrderHistoryFile();

                        historyFile = JsonConvert.DeserializeObject<OrderHistoryFile>(rawOrder);

                        _log.WriteInformationLog(string.Format("Consuming order update from queue for message ({0})", historyFile.MessageId));

                        Create(historyFile, false);
                        _conversionLogic.SaveOrderHistoryAsConfirmation(historyFile);

                        _unitOfWork.SaveChangesAndClearContext();

                        // to make sure we do not pull an order off the queue without processing it
                        // check to make sure we can still process before pulling off the queue
                        if (_keepListening) {
                            rawOrder = ReadOrderFromQueue();
                        } else {
                            rawOrder = null;
                        }
                    }
                } catch (Exception ex) {
                    KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, subject: "Exception processing Order History in Queue Service");

                    _log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }
        }

        private List<Order> LookupControlNumberAndStatus( UserSelectedContext userContext, List<EF.OrderHistoryHeader> headers ) {
            var customerOrders = new BlockingCollection<Order>();

            // Get the customer GUID to retrieve all purchase orders from commerce server
            var customerInfo = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
            var POs = _poRepo.ReadPurchaseOrderHeadersByCustomerId(customerInfo.CustomerId).ToList();

            foreach (var h in headers) {
                try {
                    Order returnOrder = null;
                    
                    returnOrder = h.ToOrder();

                    if (h.OrderSystem.Trim().Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) && h.ControlNumber.Length > 0)
                    {
                        // Check if the purchase order exists and grab it for additional information if it does
                        if (POs != null)
                        {
                            PurchaseOrder currentPo = null;
                            currentPo = POs.Where(p => p.Properties["OrderNumber"].ToString() == h.ControlNumber).FirstOrDefault();

                            if (currentPo != null)
                            {
                                returnOrder.RelatedOrderNumbers = h.RelatedControlNumber;
                                returnOrder.Status = currentPo.Status;
                                returnOrder.OrderNumber = h.ControlNumber;
                                returnOrder.IsChangeOrderAllowed = (currentPo.Properties["MasterNumber"] != null && (currentPo.Status.StartsWith("Confirmed")));
                            }
                        }
                    }
                    else
                    {
                        returnOrder.CatalogType = h.BranchId;
                        returnOrder.CatalogId = h.BranchId;
                    }

                    //if ((returnOrder.CatalogId != null) && (returnOrder.CatalogId.Length > 0)) LookupProductDetails(returnOrder.CatalogId, returnOrder);
                    //else LookupProductDetails(userContext.BranchId, returnOrder);

                    var invoice = _kpayInvoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, returnOrder.InvoiceNumber);
                    if (invoice != null) {
                        returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
                    }

                    if (returnOrder.ActualDeliveryTime != null) {
                        returnOrder.Status = "Delivered";
                    }

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

        /// <summary>
        /// Pulls catalogname from purchase order lineitems and sets the catalogid and catalogname on order items, then bubbles those up to set them on the overall order
        /// </summary>
        /// <param name="po">The purchase order that the items are stored in</param>
        /// <param name="returnOrder">The order that the items are being set on</param>
        /// <returns></returns>
        private void PullCatalogFromPurchaseOrderItemsToOrder(PurchaseOrder po, Order returnOrder)
        {
            //_log.WriteInformationLog("InternalOrderHistoryLogic.PullCatalogFromPurchaseOrderItemsToOrder() LineItems=" +
            //    ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]).Count);
            if (po != null && po.Properties["LineItems"] != null)
            {
                foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]))
                {
                    var item = (CS.LineItem)lineItem.Target;
                    var oitem = returnOrder.Items.Where(i => i.ItemNumber.Trim() == item.ProductId).FirstOrDefault();
                    if (oitem != null)
                    {
                        oitem.CatalogId = item.CatalogName;
                        oitem.CatalogType = _catalogLogic.GetCatalogTypeFromCatalogId(item.CatalogName);
                        //_log.WriteInformationLog("InternalOrderHistoryLogic.LookupControlNumberAndStatus() item.CatalogName=" + item.CatalogName);
                    }
                }
            }
            var catalogIds = returnOrder.Items.Select(i => i.CatalogId).Distinct().ToList();
            StringBuilder sbCatalogI = new StringBuilder();
            foreach (var catalog in catalogIds)
            {
                if (sbCatalogI.Length > 0) sbCatalogI.Append(",");
                if (catalog != null) sbCatalogI.Append(catalog.ToString());
            }
            returnOrder.CatalogId = sbCatalogI.ToString();
            var catalogTypes = returnOrder.Items.Select(i => i.CatalogType).Distinct().ToList();
            StringBuilder sbCatalogT = new StringBuilder();
            foreach (var catalog in catalogTypes)
            {
                if (sbCatalogT.Length > 0) sbCatalogT.Append(",");
                if (catalog != null) sbCatalogT.Append(catalog.ToString());
            }
            returnOrder.CatalogType = sbCatalogT.ToString();
        }

        private void DetermineCatalogNotesSpecialOrder(PurchaseOrder po, ref EF.OrderHistoryHeader header)
        {
            //_log.WriteInformationLog("InternalOrderHistoryLogic.PullCatalogFromPurchaseOrderItemsToOrder() LineItems=" +
            //    ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]).Count);
            string catalogId = null;
            string catalogType;
            if (po.Properties["LineItems"] != null)
            {
                foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]))
                {
                    var item = (CS.LineItem)lineItem.Target;
                        catalogId = item.CatalogName;
                        catalogType = _catalogLogic.GetCatalogTypeFromCatalogId(item.CatalogName);
                }
            }
            // Look for certain catalogs names or at least the start to be one of the special catalogs
            if(catalogId.IndexOf("unfi")>-1)
                header.IsSpecialOrder = true;
        }

        private void LookupProductDetails(string branchId, Order order)
        {
            if (order.Items == null) { return; }

            var products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber.Trim()).ToList());

            var productDict = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(order.Items, item => {
                var prod = productDict.ContainsKey(item.ItemNumber.Trim()) ? productDict[item.ItemNumber.Trim()] : null;
                if (prod != null) {
                    item.IsValid = true;
                    item.Name = prod.Name;
                    item.Description = prod.Description;
                    item.Pack = prod.Pack;
                    item.Size = prod.Size;
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
                    if (prod.Nutritional != null)
                    {
                        item.StorageTemp = prod.Nutritional.StorageTemp;
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

        private string ReadOrderFromQueue() {
            return _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer,
                                           Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);
        }

        private void RemoveSpecialOrderItemsFromHistory(EF.OrderHistoryHeader order) {
            // clean up any previous orders where the special order item existed
            var specialOrderInfo = order.OrderDetails.Where(currentDetail => !String.IsNullOrEmpty(currentDetail.SpecialOrderHeaderId))
                                                     .Select(d => new { HeaderId = d.SpecialOrderHeaderId, LineNumber = d.SpecialOrderLineNumber })
                                                     .Distinct()
                                                     .ToList();

            // loop through each special order item in the current order
            foreach (var specialOrderItem in specialOrderInfo) {
                // find all detail records with the current line's special order info that is not the current order
                var specialLines = _detailRepo.Read(d => d.BranchId.Equals(order.BranchId)
                                                      && d.SpecialOrderHeaderId.Equals(specialOrderItem.HeaderId)
                                                      && d.SpecialOrderLineNumber.Equals(specialOrderItem.LineNumber)
                                                      && !d.InvoiceNumber.Equals(order.InvoiceNumber))
                                              .ToList();

                // loop through each found detail record
                foreach (var line in specialLines) {
                    _detailRepo.Delete(line);

                    // check to see if there are any more records on the detail's header record
                    if (_detailRepo.Read(d => d.BranchId.Equals(line.BranchId)
                                           && d.InvoiceNumber.Equals(line.InvoiceNumber))
                                  .Any() == false) {
                        _headerRepo.Delete(h => h.BranchId.Equals(line.BranchId)
                                             && h.InvoiceNumber.Equals(line.InvoiceNumber));
                    }
                }
            }

            // this is commented out so that all updates to EF happen in one transaction for the current order
            //_unitOfWork.SaveChanges();
        }

        private void RemoveEmptyPurchaseOrder() { }

        public void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder) {
            Create(historyFile, isSpecialOrder);

            _unitOfWork.SaveChanges();
        }

        public void StopListening() {
            _keepListening = false;

            if (_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }

        public string SetLostOrder(string trackingNumber)
        {
            _log.WriteInformationLog("InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ")");
            PurchaseOrder Po = _poRepo.ReadPurchaseOrderByTrackingNumber(trackingNumber);
            //Save to Commerce Server
            if (Po != null)
            {
                com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
                client.UpdatePurchaseOrderStatus(Po.Properties["UserId"].ToString().ToGuid(), Po.Id.ToGuid(), "Lost");
                _log.WriteInformationLog(" InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ") Success");
                return "Success";
            }
            else
            {
                _log.WriteInformationLog(" InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ") Po not found");
                return "Po not found";
            }
        }

        public string CheckForLostOrders(out string sBody)
        {
            StringBuilder sbMsgSubject = new StringBuilder();
            StringBuilder sbMsgBody = new StringBuilder();
            List<string> statuses = Configuration.CheckLostOrdersStatus;
            foreach(string status in statuses)
                CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, status);
            sBody = sbMsgBody.ToString();
            if (sbMsgSubject.Length > 0) sbMsgSubject.Insert(0, "QSvc on " + Environment.MachineName + "; ");
            return sbMsgSubject.ToString();
        }

        private void CheckForLostOrdersByStatus(StringBuilder sbMsgSubject, StringBuilder sbMsgBody, string qStatus)
        {
            List<PurchaseOrder> Pos = _poRepo.GetPurchaseOrdersByStatus(qStatus);
            StringBuilder sbAppendSubject;
            StringBuilder sbAppendBody;
            BuildAlertStringsForLostPurchaseOrders(out sbAppendSubject, out sbAppendBody, Pos, qStatus);
            if (sbAppendSubject.Length > 0)
            {
                if (sbMsgSubject.Length > 0) sbMsgSubject.Append(", ");
                sbMsgSubject.Append(sbAppendSubject.ToString());
            }
            if (sbAppendBody.Length > 0)
            {
                if (sbMsgBody.Length > 0) sbMsgBody.Append("\n\n");
                sbMsgBody.Append(sbAppendBody.ToString());
            }
        }

        private void BuildAlertStringsForLostPurchaseOrders(out StringBuilder sbSubject, out StringBuilder sbBody, List<PurchaseOrder> Pos, string qStatus)
        {
            sbSubject = new StringBuilder();
            sbBody = new StringBuilder();
            if ((Pos != null) && (Pos.Count > 0))
            {
                int count = 0;
                sbBody.Clear();
                DateTime now = DateTime.Now.AddMinutes(-10);
                
                foreach (PurchaseOrder po in Pos)
                {
                    //string sCreated = po.Properties["DateCreated"].ToString();
                    DateTime created = DateTime.Parse(po.Properties["DateCreated"].ToString());
                    //// only if they've been created more than 10 minutes ago in the query status
                    if (created < now)
                    {
                        count++;
                        sbSubject.Clear();
                        sbSubject.Append(count + " POs in a " + qStatus + " status for more than 10 minutes.");
                        if (sbBody.Length == 0) sbBody.Append("Purchase Order Details:\n");
                        sbBody.Append("* PO");
                        sbBody.Append(" for ");
                        sbBody.Append(po.Properties["CustomerId"].ToString());
                        sbBody.Append("-");
                        sbBody.Append(po.Properties["BranchId"].ToString().ToUpper());
                        //sbBody.Append(" with cart ");
                        //sbBody.Append(po.Properties["Name"].ToString());
                        sbBody.Append(" with tracking ");
                        sbBody.Append(po.Properties["OrderNumber"].ToString());
                        sbBody.Append(" last modified");
                        sbBody.Append(" on " + created.ToString("MM-dd-yyyy hh:mm tt"));
                        sbBody.Append(" in status " + po.Properties["Status"].ToString());
                        sbBody.Append(".\n");
                    }
                }
            }
        }


        public void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber) {
            var header = _headerRepo.ReadByConfirmationNumber(childOrderNumber, "B").FirstOrDefault();

            if(header != null){
                header.RelatedControlNumber = parentOrderNumber;

                _headerRepo.Update(header);

                _unitOfWork.SaveChanges();
            }
        }

        #endregion
    }
}

