using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using CommerceServer.Core;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.com.benekeith.FoundationService;
using KeithLink.Svc.Impl.Helpers;

using CommerceRelationship = CommerceServer.Foundation.CommerceRelationship;
using CommerceRelationshipList = CommerceServer.Foundation.CommerceRelationshipList;
using CS = KeithLink.Svc.Core.Models.Generated;
using OrderHistoryDetail = KeithLink.Svc.Core.Models.Orders.History.OrderHistoryDetail;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderLogicImpl : IOrderLogic {
        #region ctor
        public OrderLogicImpl(IPurchaseOrderRepository purchaseOrderRepository, ICatalogLogic catalogLogic, INotesListLogic notesLogic, ICacheRepository cache,
                              IOrderQueueLogic orderQueueLogic, IPriceLogic priceLogic, IEventLogRepository eventLogRepository, IShipDateRepository shipRepo,
                              ICustomerRepository customerRepository, IOrderHistoryHeaderRepsitory orderHistoryRepository,
                              IKPayInvoiceRepository kpayInvoiceRepository,
                              IOrderedFromListRepository order2ListRepo) {
            _cache = cache;
            _catalogLogic = catalogLogic;
            _customerRepository = customerRepository;
            _log = eventLogRepository;
            _notesLogic = notesLogic;
            _order2ListRepo = order2ListRepo;
            _historyHeaderRepo = orderHistoryRepository;
            _invoiceRepository = kpayInvoiceRepository;
            _orderQueueLogic = orderQueueLogic;
            _priceLogic = priceLogic;
            _shipRepo = shipRepo;
            _poRepo = purchaseOrderRepository;
        }
        #endregion

        #region attributes
        private readonly ICacheRepository _cache;
        private readonly ICatalogLogic _catalogLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventLogRepository _log;
        private readonly INotesListLogic _notesLogic;
        private readonly IOrderHistoryHeaderRepsitory _historyHeaderRepo;
        private readonly IKPayInvoiceRepository _invoiceRepository;
        private readonly IOrderQueueLogic _orderQueueLogic;
        private readonly IPriceLogic _priceLogic;
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IShipDateRepository _shipRepo;
        private readonly IOrderedFromListRepository _order2ListRepo;
        #endregion

        #region methods
        public bool IsSubmitted(UserProfile user, UserSelectedContext catalogInfo, string orderNumber) {
            return OrderSubmissionHelper.CheckOrderBlock(user, catalogInfo, null, orderNumber, _poRepo, _historyHeaderRepo, _cache);
        }

        public NewOrderReturn CancelOrder(UserProfile userProfile, UserSelectedContext catalogInfo, Guid commerceId) {
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            BEKFoundationServiceClient client = new BEKFoundationServiceClient();
            string newOrderNumber = client.CancelPurchaseOrder(customer.CustomerId, commerceId);
            CS.PurchaseOrder order = _poRepo.ReadPurchaseOrder(customer.CustomerId, newOrderNumber);
            _orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.DeleteOrder, null);
            return new NewOrderReturn {OrderNumber = newOrderNumber};
        }

        private void ClearEtaInformation(Order order) {
            order.EstimatedDeliveryTime = null;
        }

        /// <summary>
        ///     Pulls orderhistory orders for that same customer within a few minutes (either before or after) and calls those
        ///     related (stores the order number and invoice number) in the order
        /// </summary>
        /// <param name="po">The purchase order that the items are stored in</param>
        /// <param name="returnOrder">The order that the items are being set on</param>
        /// <param name="headers">The reference to order history headers if we have it</param>
        /// <returns></returns>
        private void FindOrdersRelatedToPurchaseOrder(CS.PurchaseOrder po, Order returnOrder, OrderHistoryHeader thisOrder, List<OrderHistoryHeader> headers) {
            string customerNumber = po.Properties["CustomerId"].ToString();
            string branchId = po.Properties["BranchId"].ToString();
            string orderNumber = po.Properties["OrderNumber"].ToString();
            //string controlNumber = po.Properties["OrderNumber"].ToString();

            if (headers == null) {
                headers = _historyHeaderRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                       h.CustomerNumber.Equals(customerNumber) &&
                                                       h.RelatedControlNumber == orderNumber)
                                            .ToList();
            }

            StringBuilder sbRelatedOrders = new StringBuilder();
            StringBuilder sbRelatedInvoices = new StringBuilder();

            foreach (OrderHistoryHeader item in headers) {
                if (sbRelatedOrders.Length > 0) {
                    sbRelatedOrders.Append(",");
                }
                if (sbRelatedInvoices.Length > 0) {
                    sbRelatedInvoices.Append(",");
                }

                sbRelatedOrders.Append(item.ControlNumber);
                sbRelatedInvoices.Append(item.InvoiceNumber);
            }

            returnOrder.RelatedOrderNumbers = sbRelatedOrders.ToString();
            returnOrder.RelatedInvoiceNumbers = sbRelatedInvoices.ToString();
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            OrderHistoryHeader myOrder = _historyHeaderRepo.Read(h => h.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                      (h.InvoiceNumber.Equals(invoiceNumber) || h.ControlNumber.Equals(invoiceNumber)),
                                                                 d => d.OrderDetails)
                                                           .FirstOrDefault();
            CS.PurchaseOrder po = null;

            Order returnOrder = null;

            UserSelectedContext context = null;

            if (myOrder == null) {
                po = _poRepo.ReadPurchaseOrderByTrackingNumber(invoiceNumber);

                if (po == null) {
                    //throw new Exception("An order with invoice #" + invoiceNumber + " is not able to be selected in this data.");
                    // No Order exists, return null
                    return null;
                }
                context = new UserSelectedContext {BranchId = branchId, CustomerId = po.CustomerName};
                returnOrder = po.ToOrder();
                PullCatalogFromPurchaseOrderItemsToOrder(po, returnOrder);
            } else {
                returnOrder = myOrder.ToOrder();
                context = new UserSelectedContext {BranchId = branchId, CustomerId = myOrder.CustomerNumber};

                if (myOrder.OrderSystem.Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) &&
                    myOrder.ControlNumber.Length > 0) {
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
                    Invoice invoice = _invoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(myOrder.BranchId), myOrder.CustomerNumber, myOrder.InvoiceNumber);
                    if (invoice != null) {
                        returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error looking up invoice when trying to get order:  " + ex.Message + ex.StackTrace);
                }
            }

            if (returnOrder.CatalogId == null) {
                returnOrder.CatalogId = branchId;
            }

            LookupProductDetails(returnOrder.CatalogId, returnOrder);

            if (po != null) {
                returnOrder.IsChangeOrderAllowed = po.Properties["MasterNumber"] != null && po.Status.StartsWith("Confirmed");
            }

            if (returnOrder.Status == "Submitted" &&
                returnOrder.Items != null) {
                //Set all item status' to Pending. This is kind of a hack, but the correct fix will require more effort than available at the moment. The Status/Mainframe status changes are what's causing this issue
                foreach (OrderLine item in returnOrder.Items) {
                    item.MainFrameStatus = "Pending";
                }
            }

            GiveLinkForSimilarItems(returnOrder);
            UpdateOrderForShipDate(context, null, returnOrder);
            returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);

            try {
                returnOrder.ListId = _order2ListRepo.Read(returnOrder.OrderNumber)
                                                    .ListId;
            } catch {}

            return returnOrder;
        }

        private void GiveLinkForSimilarItems(Order returnOrder) {
            foreach (OrderLine item in returnOrder.Items) {
                //item.RequestDSRContact = "/messaging/RequestDSRContact?itemnumber=" + item.ItemNumber;
                StringBuilder sbSimilar = new StringBuilder();
                StringBuilder sbSimilarItem = new StringBuilder();
                sbSimilarItem.Append(item.CategoryName);
                sbSimilarItem.Replace(" ", "%20");
                sbSimilar.Append("/catalog/search/" + sbSimilarItem + "/products?dept=&from=0&sdir=asc&size=50");
                item.GetSimilarItems = sbSimilar.ToString();
            }
        }

        public List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(customerInfo.CustomerId, customerInfo.BranchId);

            List<Order> oh = GetOrderHistoryHeadersForDateRange(customerInfo, startDate, endDate);
            List<CS.PurchaseOrder> cs = _poRepo.ReadPurchaseOrderHeadersInDateRange(customer.CustomerId, customerInfo.CustomerId, startDate, endDate);

            return MergeOrderLists(cs.Select(o => o.ToOrder())
                                     .ToList(), oh);
        }

        private List<Order> GetOrderHistoryHeadersForDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            int daysDiff = (endDate - startDate).Days;
            IEnumerable<string> dateRange = Enumerable.Range(0, daysDiff + 1)
                                                      .Select(dt => startDate.AddDays(dt)
                                                                             .ToLongDateFormat());

            List<OrderHistoryHeader> headers = _historyHeaderRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                            h.CustomerNumber.Equals(customerInfo.CustomerId) &&
                                                                            dateRange.Contains(h.DeliveryDate),
                                                                       i => i.OrderDetails)
                                                                 .ToList();
            return LookupControlNumberAndStatus(customerInfo, headers);
        }

        private List<Order> GetOrderHistoryOrders(UserSelectedContext customerInfo) {
            List<OrderHistoryHeader> headers = _historyHeaderRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                            h.CustomerNumber.Equals(customerInfo.CustomerId),
                                                                       d => d.OrderDetails)
                                                                 .ToList();
            return LookupControlNumberAndStatus(customerInfo, headers);
        }

        public List<Order> GetOrders(Guid userId, UserSelectedContext customerInfo) {
            return GetOrderHistoryOrders(customerInfo)
                    .OrderByDescending(o => o.InvoiceNumber)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToList();
        }

        public PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging) {
            Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(false);
            IQueryable<OrderHistoryHeader> headersQry = _historyHeaderRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                                     h.CustomerNumber.Equals(customerInfo.CustomerId),
                                                                                d => d.OrderDetails);
            headersQry = ApplyPagingToQuery(paging, headersQry);
            stopWatch.Read(_log, "GetPagedOrders - Total time to get history headers and details query");

            List<OrderHistoryHeader> headers = headersQry.ToList();
            stopWatch.Read(_log, "GetPagedOrders - Total time to get history headers and details list");

            IQueryable<Order> data = LookupControlNumberAndStatus(customerInfo, headers)
                    .AsQueryable();
            stopWatch.Read(_log, "GetPagedOrders - Total time to get lookupcontrolnumberandstatus asqueryable");

            paging.From = 0;
            PagedResults<Order> pagedData = data.GetPage(paging);
            stopWatch.Read(_log, "GetPagedOrders - Total time to get page");

            pagedData.TotalResults = _historyHeaderRepo.GetCustomerOrderHistoryHeaders(customerInfo.BranchId, customerInfo.CustomerId)
                                                       .Count();
            stopWatch.Read(_log, "GetPagedOrders - Total time total count");

            return pagedData;
        }

        private IQueryable<OrderHistoryHeader> ApplyPagingToQuery(PagingModel paging, IQueryable<OrderHistoryHeader> headersQry) {
            if (paging != null) {
                headersQry = AddSortToPagedQuery(paging, headersQry);

                if (paging.Sort != null && paging.From != null)
                {
                    headersQry = headersQry.Skip(paging.From.Value);
                }

                if (paging.Size != null) {
                    headersQry = headersQry.Take(paging.Size.Value);
                } else {
                    headersQry = headersQry.Take(10);
                }

            } else {
                headersQry = headersQry.OrderByDescending(h => h.CreatedUtc);
                headersQry = headersQry.Take(6);
            }

            return headersQry;
        }

        private IQueryable<OrderHistoryHeader> AddSortToPagedQuery(PagingModel paging, IQueryable<OrderHistoryHeader> headersQry) {
            if (paging.Sort != null) {
                switch (paging.Sort[0].Field) {
                    case "invoicenumber":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.InvoiceNumber);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.InvoiceNumber);
                        }
                        break;
                    case "createddate":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.CreatedUtc);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.CreatedUtc);
                        }
                        break;
                    case "status":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.OrderStatus);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.OrderStatus);
                        }
                        break;
                    case "deliverydate":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.DeliveryDate);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.DeliveryDate);
                        }
                        break;
                    case "ordertotal":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.OrderSubtotal);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.OrderSubtotal);
                        }
                        break;
                    case "invoicestatus":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.OrderStatus);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.OrderStatus);
                        }
                        break;
                    case "ponumber":
                        if (paging.Sort[0].Order.Equals("asc")) {
                            headersQry = headersQry.OrderBy(h => h.PONumber);
                        } else {
                            headersQry = headersQry.OrderByDescending(h => h.PONumber);
                        }
                        break;
                    default:
                        headersQry = headersQry.OrderByDescending(h => h.CreatedUtc);
                        break;
                }
                //headersQry = headersQry.Skip(paging.From.Value); // Skip can only be done with a sort
            } else {
                headersQry = headersQry.OrderByDescending(h => h.CreatedUtc);
            }

            return headersQry;
        }

        public List<OrderHeader> GetSubmittedUnconfirmedOrders() {
            BEKFoundationServiceClient client = new BEKFoundationServiceClient();

            XmlNodeList nodes = client.GetUnconfirmatedOrders()
                                      .SelectNodes("/PurchaseOrder");

            BlockingCollection<OrderHeader> orders = new BlockingCollection<OrderHeader>();

            Parallel.ForEach(nodes.Cast<XmlNode>(), p => {
                                                        OrderHeader order = new OrderHeader();

                                                        order.CustomerNumber = p.SelectNodes("WeaklyTypedProperties/WeaklyTypedProperty[@Name='CustomerId']")[0].Attributes["Value"].Value;
                                                        order.Branch = p.SelectNodes("WeaklyTypedProperties/WeaklyTypedProperty[@Name='BranchId']")[0].Attributes["Value"].Value;
                                                        order.ControlNumber = Convert.ToInt32(p.Attributes["TrackingNumber"].Value);
                                                        order.OrderCreateDateTime = Convert.ToDateTime(p.Attributes["Created"].Value)
                                                                                           .ToUniversalTime();

                                                        orders.Add(order);
                                                    });

            return orders.ToList();
        }

        private Guid GetUserIdForControlNumber(int controlNumber) {
            // todo move this to a common location; confirmation logic does the same thing
            DataSet searchableProperties = CommerceServerCore.GetPoManager()
                                                             .GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = CommerceServerCore.GetPoManager()
                                                                        .GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause trackingNumberClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", controlNumber.ToString("0000000.##"));

            // Create search options.
            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;

            // Perform the search.
            DataSet results = CommerceServerCore.GetPoManager()
                                                .SearchPurchaseOrders(trackingNumberClause, options);

            if (results.Tables.Count > 0 &&
                results.Tables[0].Rows.Count > 0) {
                return Guid.Parse(results.Tables[0]
                                         .Rows[0]
                                         .ItemArray[2]
                                         .ToString());
            }
            return Guid.Empty;
        }

        private List<Order> LookupControlNumberAndStatus(UserSelectedContext userContext, List<OrderHistoryHeader> headers) {
            Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(false);
            BlockingCollection<Order> customerOrders = new BlockingCollection<Order>();

            ShipDateReturn shipDatesCntnr = _shipRepo.GetShipDates(userContext);
            stopWatch.Read(_log, "LookupControlNumberAndStatus - GetShipDates");

            // Get the customer GUID to retrieve all purchase orders from commerce server
            Customer customerInfo = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
            stopWatch.Read(_log, "LookupControlNumberAndStatus - GetCustomerByCustomerNumber");
            List<CS.PurchaseOrder> POs = _poRepo.ReadPurchaseOrderHeadersByCustomerId(customerInfo.CustomerId);
            stopWatch.Read(_log, "LookupControlNumberAndStatus - ReadPurchaseOrderHeadersByCustomerId");

            foreach (OrderHistoryHeader h in headers.OrderByDescending(hdr => hdr.CreatedUtc)) {
                try {
                    Order returnOrder = null;

                    returnOrder = h.ToOrder();

                    if (h.OrderSystem.Trim()
                         .Equals(OrderSource.Entree.ToShortString(), StringComparison.InvariantCultureIgnoreCase) &&
                        h.ControlNumber.Length > 0) {
                        returnOrder.RelatedOrderNumbers = h.RelatedControlNumber;

                        // Check if the purchase order exists and grab it for additional information if it does
                        if (POs != null) {
                            CS.PurchaseOrder currentPo = null;
                            currentPo = POs.Where(p => p.Properties["OrderNumber"].ToString() == h.ControlNumber)
                                           .FirstOrDefault();

                            if (currentPo != null) {
                                returnOrder.Status = currentPo.Status;
                                returnOrder.IsChangeOrderAllowed = currentPo.Properties["MasterNumber"] != null && currentPo.Status.StartsWith("Confirmed");
                            }
                        }
                    } else {
                        returnOrder.CatalogType = h.BranchId;
                        returnOrder.CatalogId = h.BranchId;
                    }

                    //if ((returnOrder.CatalogId != null) && (returnOrder.CatalogId.Length > 0)) LookupProductDetails(returnOrder.CatalogId, returnOrder);
                    //else LookupProductDetails(userContext.BranchId, returnOrder);

                    Invoice invoice = _invoiceRepository.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, returnOrder.InvoiceNumber);
                    if (invoice != null) {
                        returnOrder.InvoiceStatus = EnumUtils<InvoiceStatus>.GetDescription(invoice.DetermineStatus());
                    }

                    if (returnOrder.ActualDeliveryTime != null) {
                        returnOrder.Status = "Delivered";
                        returnOrder.IsChangeOrderAllowed = false;
                    }

                    if (returnOrder.IsChangeOrderAllowed) {
                        UpdateOrderForShipDate(userContext, shipDatesCntnr.ShipDates, returnOrder);
                    }

                    if (returnOrder.Items != null) {
                        if (h.OrderSubtotal > 0) {
                            returnOrder.OrderTotal = (double) h.OrderSubtotal;
                        } else {
                            returnOrder.OrderTotal = returnOrder.Items.Sum(i => i.LineTotal);
                        }
                    }

                    customerOrders.Add(returnOrder);
                    stopWatch.Read(_log, "LookupControlNumberAndStatus - Add Order");
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error proceesing order history for order: " + h.InvoiceNumber + ".  " + ex.StackTrace);
                }
            }

            stopWatch.Read(_log, "LookupControlNumberAndStatus - Finished Processing");
            return customerOrders.ToList();
        }

        private void LookupProductDetails(string branchId, Order order) {
            if (order.Items == null) {
                return;
            }

            ProductsReturn products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber.Trim())
                                                                                    .ToList());

            Dictionary<string, Product> productDict = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(order.Items, item => {
                                              Product prod = productDict.ContainsKey(item.ItemNumber.Trim()) ? productDict[item.ItemNumber.Trim()] : null;
                                              if (prod != null) {
                                                  item.IsValid = true;
                                                  item.Name = prod.Name;
                                                  item.Description = prod.Description;
                                                  item.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                                              prod.Name,
                                                                              prod.ItemNumber,
                                                                              prod.BrandExtendedDescription,
                                                                              prod.ItemClass,
                                                                              prod.PackSize);
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
                                                  item.CategoryCode = prod.CategoryCode;
                                                  item.SubCategoryCode = prod.SubCategoryCode;
                                                  item.CategoryName = prod.CategoryName;
                                                  item.UPC = prod.UPC;
                                                  item.VendorItemNumber = prod.VendorItemNumber;
                                                  item.Cases = prod.Cases;
                                                  item.Kosher = prod.Kosher;
                                                  item.ManufacturerName = prod.ManufacturerName;
                                                  item.ManufacturerNumber = prod.ManufacturerNumber;
                                                  item.AverageWeight = prod.AverageWeight;
                                                  if (prod.Nutritional != null) {
                                                      item.StorageTemp = prod.Nutritional.StorageTemp;
                                                      item.Nutritional = new Nutritional {
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

        private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, Order order, List<ListItemModel> notes) {
            if (order.Items == null) {
                return;
            }

            ProductsReturn products = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, order.Items.Select(l => l.ItemNumber)
                                                                                                .ToList());
            PriceReturn pricing = _priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);

            Parallel.ForEach(order.Items, item => {
                                              Product prod = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber))
                                                                     .FirstOrDefault();
                                              Price price = pricing.Prices.Where(p => p.ItemNumber.Equals(item.ItemNumber))
                                                                   .FirstOrDefault();
                                              IEnumerable<ListItemModel> note = notes.Where(n => n.ItemNumber.Equals(item.ItemNumber));
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
                                                  item.CategoryCode = prod.CategoryCode;
                                                  item.SubCategoryCode = prod.SubCategoryCode;
                                                  item.CategoryName = prod.CategoryName;
                                                  item.UPC = prod.UPC;
                                                  item.VendorItemNumber = prod.VendorItemNumber;
                                                  item.Cases = prod.Cases;
                                                  item.Kosher = prod.Kosher;
                                                  item.ManufacturerName = prod.ManufacturerName;
                                                  item.ManufacturerNumber = prod.ManufacturerNumber;
                                                  item.AverageWeight = prod.AverageWeight;
                                                  item.Nutritional = new Nutritional {
                                                      CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
                                                      GrossWeight = prod.Nutritional.GrossWeight,
                                                      HandlingInstructions = prod.Nutritional.HandlingInstructions,
                                                      Height = prod.Nutritional.Height,
                                                      Length = prod.Nutritional.Length,
                                                      Ingredients = prod.Nutritional.Ingredients,
                                                      Width = prod.Nutritional.Width
                                                  };
                                              }
                                              if (price != null) {
                                                  item.PackagePrice = price.PackagePrice.ToString();
                                                  item.CasePrice = price.CasePrice.ToString();
                                              }
                                              if (note.Any()) {
                                                  item.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber))
                                                                    .Select(i => i.Notes)
                                                                    .FirstOrDefault();
                                              }
                                          });
        }

        private List<Order> MergeOrderLists(List<Order> commerceServerOrders, List<Order> orderHistoryOrders) {
            BlockingCollection<Order> mergedOrdeList = new BlockingCollection<Order>();

            Parallel.ForEach(orderHistoryOrders, ohOrder => { mergedOrdeList.Add(ohOrder); });

            Parallel.ForEach(commerceServerOrders, csOrder => {
                                                       if (mergedOrdeList.Where(o => o.InvoiceNumber.Equals(csOrder.InvoiceNumber))
                                                                         .Count() == 0) {
                                                           mergedOrdeList.Add(csOrder);
                                                       }
                                                   });

            return mergedOrdeList.ToList();
        }

        /// <summary>
        ///     Pulls catalogname from purchase order lineitems and sets the catalogid and catalogname on order items, then bubbles
        ///     those up to set them on the overall order
        /// </summary>
        /// <param name="po">The purchase order that the items are stored in</param>
        /// <param name="returnOrder">The order that the items are being set on</param>
        /// <returns></returns>
        private void PullCatalogFromPurchaseOrderItemsToOrder(CS.PurchaseOrder po, Order returnOrder) {
            //_log.WriteInformationLog("InternalOrderHistoryLogic.PullCatalogFromPurchaseOrderItemsToOrder() LineItems=" +
            //    ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]).Count);
            if (po != null &&
                po.Properties["LineItems"] != null) {
                foreach (CommerceRelationship lineItem in (CommerceRelationshipList) po.Properties["LineItems"]) {
                    CS.LineItem item = (CS.LineItem) lineItem.Target;
                    OrderLine oitem = returnOrder.Items.Where(i => i.ItemNumber.Trim() == item.ProductId)
                                                 .FirstOrDefault();
                    if (oitem != null) {
                        oitem.CatalogId = item.CatalogName;
                        oitem.CatalogType = _catalogLogic.GetCatalogTypeFromCatalogId(item.CatalogName);
                        //_log.WriteInformationLog("InternalOrderHistoryLogic.LookupControlNumberAndStatus() item.CatalogName=" + item.CatalogName);
                    }
                }
            }
            List<string> catalogIds = returnOrder.Items.Select(i => i.CatalogId)
                                                 .Distinct()
                                                 .ToList();
            StringBuilder sbCatalogI = new StringBuilder();
            foreach (string catalog in catalogIds) {
                if (sbCatalogI.Length > 0) {
                    sbCatalogI.Append(",");
                }
                if (catalog != null) {
                    sbCatalogI.Append(catalog);
                }
            }
            returnOrder.CatalogId = sbCatalogI.ToString();
            List<string> catalogTypes = returnOrder.Items.Select(i => i.CatalogType)
                                                   .Distinct()
                                                   .ToList();
            StringBuilder sbCatalogT = new StringBuilder();
            foreach (string catalog in catalogTypes) {
                if (sbCatalogT.Length > 0) {
                    sbCatalogT.Append(",");
                }
                if (catalog != null) {
                    sbCatalogT.Append(catalog);
                }
            }
            returnOrder.CatalogType = sbCatalogT.ToString();
        }

        public Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber, bool omitDeletedItems = true) {
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);
            CS.PurchaseOrder order = _poRepo.ReadPurchaseOrder(customer.CustomerId, orderNumber);
            Order returnOrder = ToOrder(order, false);
            List<ListItemModel> notes = _notesLogic.GetNotes(userProfile, catalogInfo);

            LookupProductDetails(userProfile, catalogInfo, returnOrder, notes);

            // handel special change order logic to hide deleted line items
            if (returnOrder.IsChangeOrderAllowed && omitDeletedItems) // change order eligible - remove lines marked as 'deleted'
            {
                returnOrder.Items = returnOrder.Items.Where(x => x.ChangeOrderStatus != "deleted")
                                               .ToList();
            }
            return returnOrder;
        }

        public List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true, bool header = false, bool changeorder = false) {
            Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(false);
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);
            stopWatch.Read(_log, "ReadOrders - GetCustomerByCustomerNumber");
            List<CS.PurchaseOrder> orders = _poRepo.ReadPurchaseOrderHeadersByCustomerId(customer.CustomerId);
            stopWatch.Read(_log, "ReadOrders - ReadPurchaseOrders");
            List<Order> returnOrders = orders.Select(p => ToOrder(p, header))
                                             .ToList();
            stopWatch.Read(_log, "ReadOrders - SelectToOrder");
            List<ListItemModel> notes = _notesLogic.GetNotes(userProfile, catalogInfo);
            stopWatch.Read(_log, "ReadOrders - GetNotes");

            returnOrders.ForEach(delegate(Order order) {
                                     LookupProductDetails(userProfile, catalogInfo, order, notes);
                                     if (omitDeletedItems) {
                                         order.Items = order.Items.Where(x => x.MainFrameStatus != "deleted")
                                                            .ToList();
                                     }
                                     stopWatch.Read(_log, "ReadOrders - LookupProductDetails");
                                 });
            stopWatch.Read(_log, "Added Orders");

            if (changeorder) {
                returnOrders = returnOrders.Where(co => co.IsChangeOrderAllowed)
                                           .ToList();
            }
            stopWatch.Read(_log, "ReadOrders - if (changeorder)");
            return returnOrders.OrderByDescending(o => o.InvoiceNumber)
                               .ToList();
        }

        public List<Order> ReadOrderHistories(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true) {
            List<Order> orders = GetOrders(userProfile.UserId, catalogInfo);

            //var returnOrders = orders.Select(p => ToOrder(p)).ToList();
            List<ListItemModel> notes = _notesLogic.GetNotes(userProfile, catalogInfo);

            //return returnOrders;
            return orders;
        }

        public bool ResendUnconfirmedOrder(UserProfile userProfile, int controlNumber, UserSelectedContext catalogInfo) {
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);
            Guid userId = GetUserIdForControlNumber(controlNumber);

            string controlNumberMainFrameFormat = controlNumber.ToString("0000000.##");
            CS.PurchaseOrder order = _poRepo.ReadPurchaseOrder(customer.CustomerId, controlNumberMainFrameFormat);

            string originalOrderNumber = order.Properties["OriginalOrderNumber"].ToString();

            OrderType type = originalOrderNumber == controlNumberMainFrameFormat ? OrderType.NormalOrder : OrderType.ChangeOrder;

            _orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, controlNumberMainFrameFormat, order, type, null); // TODO, logic to compare original order number and control number

            return true;
        }

        public NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber) {
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            CS.PurchaseOrder order = _poRepo.ReadPurchaseOrder(customer.CustomerId, orderNumber); // TODO: incorporate multi user query

            List<OrderLine> items = ((CommerceRelationshipList) order.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem) l.Target))
                                                                                              .ToList();
            if (items == null ||
                items.Count == 0) {
                throw new ApplicationException("Cannot submit order with 0 line items");
            }

            BEKFoundationServiceClient client = new BEKFoundationServiceClient();
            string newOrderNumber = client.SaveOrderAsChangeOrder(customer.CustomerId, Guid.Parse(order.Id));

            OrderSubmissionHelper.StartChangeOrderBlock(orderNumber, newOrderNumber, _cache);

            // transfer prior order's listid to an entry for the changeorder
            OrderedFromList o2l = _order2ListRepo.Read(orderNumber);
            if (o2l != null &&
                o2l.ListId != null) {
                o2l.ControlNumber = newOrderNumber;
                _order2ListRepo.Write(o2l);
            }

            order = _poRepo.ReadPurchaseOrder(customer.CustomerId, newOrderNumber);

            _orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.ChangeOrder, null);

            client.CleanUpChangeOrder(customer.CustomerId, Guid.Parse(order.Id));

            return new NewOrderReturn {OrderNumber = newOrderNumber};
        }

        private Order ToOrder(CS.PurchaseOrder purchaseOrder, bool headerOnly) {
            return new Order {
                CreatedDate = purchaseOrder.Properties["DateCreated"].ToString()
                                           .ToDateTime()
                                           .Value,
                OrderNumber = purchaseOrder.Properties["OrderNumber"].ToString(),
                OrderTotal = purchaseOrder.Properties["Total"].ToString()
                                          .ToDouble()
                                          .Value,
                InvoiceNumber = purchaseOrder.Properties["MasterNumber"] == null ? string.Empty : purchaseOrder.Properties["MasterNumber"].ToString(),
                // if we have a master number (invoice #) and a confirmed or submitted status
                IsChangeOrderAllowed = purchaseOrder.Properties["MasterNumber"] != null
                                       && purchaseOrder.Status.StartsWith("Confirmed") | purchaseOrder.Status.StartsWith("Submitted"),
                Status = Regex.Replace(purchaseOrder.Status, "([a-z])([A-Z])", "$1 $2"),
                RequestedShipDate = purchaseOrder.Properties["RequestedShipDate"].ToString()
                                                 .ToDateTime()
                                                 .Value.ToLongDateFormat(),
                Items = purchaseOrder.Properties["LineItems"] == null || headerOnly ? new List<OrderLine>() : ((CommerceRelationshipList) purchaseOrder.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem) l.Target))
                                                                                                                                                                                .ToList(),
                ItemCount = purchaseOrder.Properties["LineItems"] == null ? 0 : ((CommerceRelationshipList) purchaseOrder.Properties["LineItems"]).Count,
                CommerceId = Guid.Parse(purchaseOrder.Id),
                PONumber = purchaseOrder.Properties["PONumber"] == null ? string.Empty : purchaseOrder.Properties["PONumber"].ToString()
            };
        }

        private Order ToOrder(Core.Models.Orders.History.OrderHistoryHeader orderHistoryHeader) {
            DateTime createdDateTime = new DateTime();

            if (!DateTime.TryParse(orderHistoryHeader.DeliveryDate, out createdDateTime)) {
                createdDateTime = DateTime.Now;
            }

            return new Order {
                CreatedDate = createdDateTime,
                OrderNumber = orderHistoryHeader.ControlNumber,
                OrderTotal = orderHistoryHeader.Items.Sum(l => l.SellPrice),
                InvoiceNumber = orderHistoryHeader.InvoiceNumber,
                IsChangeOrderAllowed = false,
                Status = orderHistoryHeader.OrderStatus,
                RequestedShipDate = DateTime.Now.ToLongDateFormat(),
                Items = orderHistoryHeader.Items.Select(l => ToOrderLine(l))
                                          .ToList(),
                CommerceId = Guid.Empty // could be orders from any system
            };
        }

        private OrderLine ToOrderLine(CS.LineItem lineItem) {
            return new OrderLine {
                ItemNumber = lineItem.ProductId,
                Quantity = lineItem.Quantity == null ? 0 : (short) lineItem.Quantity,
                Price = lineItem.PlacedPrice == null ? 0 : (double) lineItem.PlacedPrice,
                QuantityOrdered = lineItem.Properties["QuantityOrdered"] == null ? 0 : (int) lineItem.Properties["QuantityOrdered"],
                QantityShipped = lineItem.Properties["QuantityShipped"] == null ? 0 : (int) lineItem.Properties["QuantityShipped"],
                ChangeOrderStatus = lineItem.Status,
                SubstitutedItemNumber = lineItem.Properties["SubstitutedItemNumber"] == null ? null : (string) lineItem.Properties["SubstitutedItemNumber"],
                LineNumber = lineItem.Properties["LinePosition"] == null ? 1 : int.Parse(lineItem.Properties["LinePosition"].ToString()),
                MainFrameStatus = lineItem.Properties["MainFrameStatus"] == null ? null : (string) lineItem.Properties["MainFrameStatus"],
                Each = (bool) lineItem.Properties["Each"]
            };
        }

        private OrderLine ToOrderLine(OrderHistoryDetail lineItem) {
            return new OrderLine {
                ItemNumber = lineItem.ItemNumber,
                Quantity = (short) lineItem.ShippedQuantity,
                Price = lineItem.SellPrice,
                QuantityOrdered = lineItem.OrderQuantity,
                QantityShipped = lineItem.ShippedQuantity,
                //Status = lineItem.ItemStatus,
                SubstitutedItemNumber = !string.IsNullOrEmpty(lineItem.ReplacedOriginalItemNumber.Trim()) ? lineItem.ReplacedOriginalItemNumber :
                                            !string.IsNullOrEmpty(lineItem.SubbedOriginalItemNumber.Trim()) ? lineItem.SubbedOriginalItemNumber : string.Empty,
                MainFrameStatus = lineItem.ItemStatus,
                Each = lineItem.UnitOfMeasure == UnitOfMeasure.Package ? true : false
            };
        }

        private void UpdateExistingOrderInfo(Order order, Order existingOrder, bool deleteOmmitedItems) {
            // work through adds, deletes, changes based on item number
            foreach (OrderLine newLine in order.Items) {
                OrderLine existingLine = existingOrder.Items.Where(x => x.ItemNumber == newLine.ItemNumber)
                                                      .FirstOrDefault();
                if (existingLine != null) {
                    // compare and update if necessary
                    if (existingLine.Quantity != newLine.Quantity ||
                        existingLine.Each != newLine.Each) {
                        existingLine.Quantity = newLine.Quantity;
                        existingLine.Each = newLine.Each;
                        if (!string.IsNullOrEmpty(existingLine.MainFrameStatus)) //If this hasn't been sent to the mainframe yet, then it's still an add, not a change
                        {
                            existingLine.ChangeOrderStatus = "changed";
                        }
                    }
                } else {
                    // new line
                    existingOrder.Items.Add(new OrderLine {
                        ItemNumber = newLine.ItemNumber,
                        Quantity = newLine.Quantity,
                        Each = newLine.Each,
                        ChangeOrderStatus = "added",
                        LineNumber = existingOrder.Items.Select(li => li.LineNumber)
                                                  .Max() + 1
                    });
                }
            }
            // handle deletes
            if (deleteOmmitedItems) {
                foreach (OrderLine existingLine in existingOrder.Items) {
                    OrderLine newLine = order.Items.Where(x => x.ItemNumber == existingLine.ItemNumber)
                                             .FirstOrDefault();
                    if (newLine == null) {
                        existingLine.ChangeOrderStatus = "deleted";
                        _log.WriteInformationLog("Deleting line: " + existingLine.ItemNumber);
                    }
                }
            }
        }

        public Order UpdateOrder(UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems) {
            /*if (order.Items == null || order.ItemCount == 0)
            {
                throw new ApplicationException("Cannot submit an order with zero line items");
            }
             * */
            Customer customer = _customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            Order existingOrder = ReadOrder(user, catalogInfo, order.OrderNumber, false);
            List<ListItemModel> notes = _notesLogic.GetNotes(user, catalogInfo);

            LookupProductDetails(user, catalogInfo, order, notes);
            UpdateExistingOrderInfo(order, existingOrder, deleteOmmitedItems);

            OrderedFromList o2l = _order2ListRepo.Read(order.OrderNumber);
            if (o2l == null &&
                order.ListId != null) {
                _order2ListRepo.Write(new OrderedFromList {
                    ControlNumber = order.OrderNumber,
                    ListId = order.ListId.Value
                });
            } else if (o2l != null &&
                       o2l.ListId != order.ListId) {
                _order2ListRepo.Delete(order.OrderNumber);
                if (order.ListId != null) {
                    _order2ListRepo.Write(new OrderedFromList {
                        ControlNumber = order.OrderNumber,
                        ListId = order.ListId.Value
                    });
                }
            }

            BEKFoundationServiceClient client = new BEKFoundationServiceClient();
            List<PurchaseOrderLineItemUpdate> itemUpdates = new List<PurchaseOrderLineItemUpdate>();

            foreach (OrderLine line in existingOrder.Items) {
                //itemUpdates.Add(new com.benekeith.FoundationService.PurchaseOrderLineItemUpdate() { ItemNumber = line.ItemNumber, Quantity = line.Quantity, Status = line.Status, Catalog = catalogInfo.BranchId, Each = line.Each, CatchWeight = line.CatchWeight });
                itemUpdates.Add(new PurchaseOrderLineItemUpdate {
                    ItemNumber = line.ItemNumber,
                    Quantity = line.Quantity,
                    Status = line.ChangeOrderStatus,
                    Catalog = catalogInfo.BranchId,
                    Each = line.Each,
                    CatchWeight = line.CatchWeight
                });
            }
            string orderNumber = client.UpdatePurchaseOrder(customer.CustomerId, existingOrder.CommerceId, order.RequestedShipDate, itemUpdates.ToArray());

            return ReadOrder(user, catalogInfo, order.OrderNumber);
        }

        public Order UpdateOrderForShipDate(UserSelectedContext userContext, List<ShipDate> shipDates, Order order) {
            if (shipDates == null) {
                ShipDateReturn shipDatesCntnr = _shipRepo.GetShipDates(userContext);
                shipDates = shipDatesCntnr.ShipDates;
            }
            string requested = DateTime.Parse(order.RequestedShipDate)
                                       .ToString("yyyy-MM-dd");
            if (shipDates.Select(s => s.Date)
                         .ToList()
                         .Contains(requested)) {
                string cutofftime = shipDates.Where(s => s.Date == requested)
                                             .Select(s => s.CutOffDateTime)
                                             .First();
                if (DateTime.Now > DateTime.Parse(cutofftime)) {
                    order.IsChangeOrderAllowed = false;
                }
            } else // if there isn't a ship date matching the order's requested ship date, then we assume it can no longer be edited.
            {
                order.IsChangeOrderAllowed = false;
            }

            return order;
        }

        public Order UpdateOrderForEta(UserProfile user, Order order) {
            if (Configuration.EnableEtaForUsers.Equals("none", StringComparison.InvariantCultureIgnoreCase)
                ||
                Configuration.EnableEtaForUsers.Equals("internal_only", StringComparison.InvariantCultureIgnoreCase)
                && !ProfileHelper.IsInternalAddress(user.EmailAddress)
            ) {
                ClearEtaInformation(order);
            }
            return order;
        }

        public List<Order> UpdateOrdersForSecurity(UserProfile user, List<Order> orders) {
            Parallel.ForEach(orders, o => { UpdateOrderForEta(user, o); });
            return orders;
        }
        #endregion
    }
}