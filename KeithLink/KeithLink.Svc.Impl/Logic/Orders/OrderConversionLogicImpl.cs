﻿using CommerceServer.Core;
using CommerceServer.Core.Runtime.Orders;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderConversionLogicImpl : IOrderConversionLogic {
        #region attributes
        private readonly IEventLogRepository _log;
        private readonly IOrderHistoryHeaderRepsitory _historyRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICatalogLogic _catalogLogic;
        #endregion

        #region ctor
        public OrderConversionLogicImpl(IOrderHistoryHeaderRepsitory historyRepository, IUnitOfWork unitOfWork, IEventLogRepository logRepo, ICatalogLogic catalogLogic) {
            _historyRepo = historyRepository;
            _uow = unitOfWork;
            _catalogLogic = catalogLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        private PurchaseOrder GetCsPurchaseOrderByNumber(string controlNumber) {
            System.Data.DataSet searchableProperties = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause trackingNumberClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", controlNumber);

            // Create search options.

            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;
            // Perform the search.
            System.Data.DataSet results = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().SearchPurchaseOrders(trackingNumberClause, options);

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0) {
                try {
                    // Enumerate the results of the search.
                    Guid soldToId = Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());

                    // get the guids for the customers associated users and loop if necessary
                    PurchaseOrder po = Svc.Impl.Helpers.CommerceServerCore.GetOrderContext().GetPurchaseOrder(soldToId, controlNumber);
                    return po;
                } catch (Exception ex) {
                    _log.WriteWarningLog("Could not locate POs for user's ID. This is not an exception, just a notice.", ex);
                    return null;
                }
            } else {
                return null;
            }
        }

        public void SaveConfirmationAsOrderHistory(ConfirmationFile confFile) {
            if (confFile.Header.ConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_REJECTED_CODE, StringComparison.InvariantCultureIgnoreCase)) {
                SaveRejectedConfirmationAsOrderHistory(confFile);
            } else {
                OrderHistoryFile currentFile = confFile.ToOrderHistoryFile();

                EF.OrderHistoryHeader header = _historyRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber).FirstOrDefault();

                // second attempt to find the order, look by confirmation number
                if (header == null)
                {
                    header = _historyRepo.ReadByConfirmationNumber(currentFile.Header.ControlNumber, currentFile.Header.OrderSystem.ToShortString()).FirstOrDefault();

                    if (header != null)
                    {
                        header.InvoiceNumber = confFile.Header.InvoiceNumber;
                    }
                }

                // last ditch effort is to create a new header
                if (header == null)
                {
                    header = new EF.OrderHistoryHeader();
                    header.OrderDetails = new List<EF.OrderHistoryDetail>();
                }

                currentFile.Header.MergeWithEntity(ref header);

                foreach (OrderHistoryDetail currentDetail in currentFile.Details)
                {

                    EF.OrderHistoryDetail detail = null;

                    if (header.OrderDetails != null && header.OrderDetails.Count > 0)
                    {
                        detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();
                    }

                    if (detail == null)
                    {
                        EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
                        tempDetail.BranchId = header.BranchId;
                        tempDetail.InvoiceNumber = header.InvoiceNumber;

                        header.OrderDetails.Add(currentDetail.ToEntityFrameworkModel());
                    }
                    else {
                        currentDetail.MergeWithEntityFrameworkModel(ref detail);

                        detail.BranchId = header.BranchId;
                        detail.InvoiceNumber = header.InvoiceNumber;
                    }
                }

                GetSubtotal(header);

                //Mark missing items as deleted
                foreach (var deletedItem in header.OrderDetails.Where(d => !currentFile.Details.Any(c => c.LineNumber.Equals(d.LineNumber))).ToList())
                {
                    deletedItem.ItemDeleted = true;
                    deletedItem.OrderQuantity = 0;
                    deletedItem.ShippedQuantity = 0;
                }

                _historyRepo.CreateOrUpdate(header);
                _uow.SaveChanges();
            }
        }

        private void GetSubtotal(EF.OrderHistoryHeader header)
        {
            Order newOrder = header.ToOrder();
            LookupProductDetails(header.BranchId, newOrder);
            header.OrderSubtotal = (decimal)newOrder.Items.Sum(i => i.LineTotal);
        }

        private void LookupProductDetails(string branchId, Order order)
        {
            if (order.Items == null) { return; }

            var products = _catalogLogic.GetProductsByIds(branchId, order.Items.Select(l => l.ItemNumber.Trim()).ToList());

            var productDict = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(order.Items, item => {
                var prod = productDict.ContainsKey(item.ItemNumber.Trim()) ? productDict[item.ItemNumber.Trim()] : null;
                if (prod != null)
                {
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
                }
                //if (price != null) {
                //    item.PackagePrice = price.PackagePrice.ToString();
                //    item.CasePrice = price.CasePrice.ToString();
                //}
            });

        }
        public void SaveOrderHistoryAsConfirmation(OrderHistoryFile histFile) {
            if (histFile.Header.OrderSystem == Core.Enumerations.Order.OrderSource.Entree) {
                ConfirmationFile confirmation = histFile.ToConfirmationFile();
                PurchaseOrder po = GetCsPurchaseOrderByNumber(histFile.Header.ControlNumber);

                if (po != null) {
                    // need to save away pre and post status info, then if different, add something to the messaging
                    LineItem[] currLineItems = new LineItem[po.LineItemCount];
                    LineItem[] origLineItems = new LineItem[po.LineItemCount];
                    po.OrderForms[0].LineItems.CopyTo(currLineItems, 0);
                    po.OrderForms[0].LineItems.CopyTo(origLineItems, 0);
                    string originalStatus = po.Status;

                    SetCsLineInfo(currLineItems, confirmation);

                    SetCsHeaderInfo(confirmation, po, currLineItems);

                    po.Save();
                }
            }
        }

        private void SaveRejectedConfirmationAsOrderHistory(ConfirmationFile confFile) {
            //Assume we will only get rejections for Entree Orders
            Core.Enumerations.Order.OrderSource orderSource = Core.Enumerations.Order.OrderSource.Entree;
            EF.OrderHistoryHeader header = _historyRepo.ReadByConfirmationNumber(confFile.Header.ConfirmationNumber, orderSource.ToShortString()).FirstOrDefault();
            
            if (header != null) {
                header.OrderStatus = Constants.CONFIRMATION_HEADER_REJECTED_CODE;
                header.InvoiceNumber = Constants.CONFIRMATION_HEADER_REJECTED_STATUS;

                _historyRepo.CreateOrUpdate(header);
                _uow.SaveChanges();
            }
        }

        private string SetCsHeaderInfo(ConfirmationFile confirmation, PurchaseOrder po, LineItem[] lineItems) {
            string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
            if (trimmedConfirmationStatus == Constants.CONFIRMATION_HEADER_CONFIRMED_CODE) { // if confirmation status is blank, then look for exceptions across all line items, not just those in the change order
                string origOrderNumber = (string)po[Constants.CS_PURCHASE_ORDER_ORIGINAL_ORDER_NUMBER];
                string currOrderNumber = po.TrackingNumber;
                bool isChangeOrder = origOrderNumber != currOrderNumber;
                SetCsPoStatusFromLineItems(po, lineItems, isChangeOrder);
            } else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_IN_PROCESS_CODE)) {
                po.Status = Constants.CONFIRMATION_HEADER_IN_PROCESS_STATUS;
            } else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_INVOICED_CODE)) {
                po.Status = Constants.CONFIRMATION_HEADER_INVOICED_STATUS;
            } else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_DELETED_CODE)) {
                po.Status = Constants.CONFIRMATION_HEADER_DELETED_STATUS;
            } else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_REJECTED_CODE)) {
                po.Status = Constants.CONFIRMATION_HEADER_REJECTED_STATUS;
            }

            po[Constants.CS_PURCHASE_ORDER_MASTER_NUMBER] = confirmation.Header.InvoiceNumber; // read this from the confirmation file

            return trimmedConfirmationStatus;
        }

        private void SetCsLineInfo(LineItem[] lineItems, ConfirmationFile confirmation) {
            foreach (var detail in confirmation.Detail) {
                // match incoming line items to CS line items
                int linePosition = Convert.ToInt32(detail.RecordNumber);

                LineItem orderFormLineItem = lineItems.Where(x => (int)x["LinePosition"] == (linePosition)).FirstOrDefault();
                //LineItem orderFormLineItem = lineItems.Where(x => x.ProductId == detail.ItemNumber).FirstOrDefault();

                if (orderFormLineItem != null)
                {
                    SetCsLineItemInfo(orderFormLineItem, detail.QuantityOrdered, detail.QuantityShipped, detail.DisplayStatus(), detail.ItemNumber, detail.SubstitutedItemNumber(orderFormLineItem), 
                        detail.ItemStatus);
                }
                else { }
            }
        }

        private void SetCsLineItemInfo(LineItem orderFormLineItem, int quantityOrdered, int quantityShipped, string displayStatus, string currentItemNumber, string substitutedItemNumber, string itemStatus) {
            orderFormLineItem["QuantityOrdered"] = quantityOrdered;
            orderFormLineItem["QuantityShipped"] = quantityShipped;
            if (itemStatus.Length > 0)
            {
                switch (itemStatus)
                {
                    case Constants.CONFIRMATION_DETAIL_ITEM_STATUS_INVALID:
                        displayStatus = Constants.CONFIRMATION_DETAIL_ITEM_STATUS_INVALID_DESCRIPTION;
                        break;
                    case Constants.CONFIRMATION_DETAIL_ITEM_STATUS_DELETE:
                        displayStatus = Constants.CONFIRMATION_DETAIL_ITEM_STATUS_DELETE_DESCRIPTION;
                        break;
                    case Constants.CONFIRMATION_DETAIL_ITEM_STATUS_NOT_FOUND:
                        displayStatus = Constants.CONFIRMATION_DETAIL_ITEM_STATUS_NOT_FOUND_DESCRIPTION;
                        break;
                }
            }
            orderFormLineItem["MainFrameStatus"] = displayStatus;
            orderFormLineItem["SubstitutedItemNumber"] = substitutedItemNumber;
            orderFormLineItem.ProductId = currentItemNumber;
        }

        private void SetCsPoStatusFromLineItems(PurchaseOrder po, LineItem[] lineItems, bool isChangeOrder) {
            if (lineItems.Any(x => ((string)x[Constants.CS_LINE_ITEM_MAIN_FRAME_STATUS]) != Constants.CONFIRMATION_DETAIL_FILLED_STATUS)) { // exceptions
                if (isChangeOrder)
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_EXCEPTIONS_STATUS;
                else
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_EXCEPTIONS_STATUS;
            } else { // no exceptions
                if (isChangeOrder) {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_STATUS;
                } else {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_STATUS;
                }
            }
        }
        #endregion
    }
}
