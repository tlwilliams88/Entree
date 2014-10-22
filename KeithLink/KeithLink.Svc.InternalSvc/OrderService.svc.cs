using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Common.Core.Extensions;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Core.Orders;
using CommerceServer.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PipelineService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select PipelineService.svc or PipelineService.svc.cs at the Solution Explorer and start debugging.
	public class OrderService : IOrderService
	{
		public OrderService()
		{
			
		}

        static OrderContext orderContext = null;
        static OrderSiteAgent ordersAgent = null;
        static OrderManagementContext context = null;
        static PurchaseOrderManager manager = null;

		public bool OrderConfirmation(ConfirmationFile confirmation)
		{
            LoadOrderContext();

            if (String.IsNullOrEmpty(confirmation.Header.ConfirmationNumber) || String.IsNullOrEmpty(confirmation.Header.ConfirmationStatus)
                || String.IsNullOrEmpty(confirmation.Header.InvoiceNumber))
                throw new ApplicationException("confirmation number, status and invoice number are required");
            
            var poNum = confirmation.Header.ConfirmationNumber;
            PurchaseOrder po = GetCsPurchaseOrderByNumber(poNum);

            if (po == null)
            {
                // if no PO, silently ignore?  could be the case if multiple control numbers out at once...
            }
            else
            {
                string trimmedConfirmationStatus = SetCsHeaderInfo(confirmation, po);

                LineItem[] lineItems = new LineItem[po.OrderForms.Count];
                po.OrderForms[0].LineItems.CopyTo(lineItems, 0);

                SetCsLineInfo(trimmedConfirmationStatus, lineItems, GetCsLineUpdateInfo(confirmation));
                po.Save();
            }
			return true;
		}

        private static void LoadOrderContext()
        {
            if (orderContext == null)
            {
                var siteName = System.Configuration.ConfigurationManager.AppSettings["CS_Sitename"].ToString();
                ordersAgent = new OrderSiteAgent(siteName);
                context = OrderManagementContext.Create(ordersAgent);
                manager = context.PurchaseOrderManager;
                orderContext = CommerceServer.Core.Runtime.Orders.OrderContext.Create(siteName);
            }
        }

        private static PurchaseOrder GetCsPurchaseOrderByNumber(string poNum)
        {
            System.Data.DataSet searchableProperties = manager.GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause trackingNumberClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", poNum);

            // Create search options.

            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;
            // Perform the search.
            System.Data.DataSet results = manager.SearchPurchaseOrders(trackingNumberClause, options);

            // Enumerate the results of the search.
            Guid soldToId = Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());

            // get the guids for the customers associated users and loop if necessary
            PurchaseOrder po = orderContext.GetPurchaseOrder(soldToId, poNum);
            return po;
        }

        private static void SetCsLineInfo(string trimmedConfirmationStatus, LineItem[] lineItems, List<CsOrderLineUpdateInfo> confirmationDetail)
        {
            foreach (var detail in confirmationDetail)
            {
                // match up to incoming line items to CS line items
                LineItem orderFormLineItem = lineItems.Where(x => x.Index == detail.RecordNumber).FirstOrDefault();
                string confirmationStatus = detail.MainFrameStatus.Trim().ToUpper();
                if (String.IsNullOrEmpty(confirmationStatus))
                {
                    orderFormLineItem["MainFrameStatus"] = "Filled";
                }
                if (confirmationStatus == "P") // partial ship
                {
                    orderFormLineItem.BackorderQuantity = detail.QuantityOrdered - detail.QuantityShipped;
                    orderFormLineItem["MainFrameStatus"] = "Partially Shipped";
                }
                else if (confirmationStatus == "O") // out of stock
                {
                    orderFormLineItem.BackorderQuantity = detail.QuantityOrdered - detail.QuantityShipped;
                    orderFormLineItem["MainFrameStatus"] = "Out of Stock";
                }
                else if (confirmationStatus == "R") // item replaced
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Replaced";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "Z") // item replaced, but replacement currently out of stock
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Replaced, Out of Stock";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "T") // Item replaced, partial fill
                {
                    orderFormLineItem["MainFrameStatus"] = "Partially Shipped, Item Replaced";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "S") // item subbed
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Subbed";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
            }
        }

        private static string SetCsHeaderInfo(ConfirmationFile confirmation, PurchaseOrder po)
        {
            // get header status into CS
            // values are " ", "P", "I", "D" = " " open, "P" Processing, "I" Invoicing, "D" Delete
            string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
            if (String.IsNullOrEmpty(trimmedConfirmationStatus))
            {
                po.Status = "NewOrder";
            }
            else if (trimmedConfirmationStatus.Equals("P"))
            {
                po.Status = "Submitted";
            }
            else if (trimmedConfirmationStatus.Equals("I"))
            {
                po.Status = "InProcess";
            }
            else if (trimmedConfirmationStatus.Equals("D"))
            {
                po.Status = "Cancelled";
            }


            po["MasterNumber"] = confirmation.Header.InvoiceNumber; // read this from the confirmation file

            return trimmedConfirmationStatus;
        }

        List<CsOrderLineUpdateInfo> GetCsLineUpdateInfo(ConfirmationFile confirmationFile)
        {
            List<CsOrderLineUpdateInfo> ret = new List<CsOrderLineUpdateInfo>();
            foreach (var detail in confirmationFile.Detail)
            {
                CsOrderLineUpdateInfo line = new CsOrderLineUpdateInfo()
                {
                    MainFrameStatus = detail.ConfirmationMessage.Trim(),
                    SubstitueItemNumber = detail.ItemNumber,
                    QuantityOrdered = detail.QuantityOrdered,
                    QuantityShipped = detail.QuantityShipped,
                    RecordNumber = int.Parse(detail.RecordNumber)
                };
                ret.Add(line);
            }
            return ret;
        }
	}

    class CsOrderLineUpdateInfo
    {    
        public string MainFrameStatus { get; set; }
        public string SubstitueItemNumber { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityShipped { get; set; }
        public int RecordNumber { get; set; }
    }
}
