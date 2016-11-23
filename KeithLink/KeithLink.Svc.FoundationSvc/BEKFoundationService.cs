using KeithLink.Common.Impl.Repository.Logging;
using KeithLink.Svc.FoundationSvc.Interface;

using CommerceServer.Core;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Foundation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Reflection;
using System.Xml;

namespace KeithLink.Svc.FoundationSvc
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BEKFoundationService : OperationService, IBEKFoundationService
    {
        #region attributes
        private static string applicationNameForLogging = "Entree Foundation Service";
        #endregion

        #region methods
        public string CancelPurchaseOrder(Guid userId, Guid orderId) {
            try {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                po.Status = "Cancelled";
                po.TrackingNumber = GetNextControlNumber();
                po.Save();
                return po.TrackingNumber;
            } catch (Exception ex) {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CancelPurchaseOrder: ", ex);

                throw ex;
            }
        }

        public void CleanUpChangeOrder(Guid userId, Guid cartId) {
            try {
                PurchaseOrder po = GetPurchaseOrder(userId, cartId);

                List<LineItem> lineItemIndexesToRemove = new List<LineItem>();

                foreach (LineItem li in po.OrderForms[0].LineItems) {
                    if (li.Status == "deleted") {
                        lineItemIndexesToRemove.Add(li);
                    }
                    li.Status = string.Empty;
                }

                foreach (LineItem item in lineItemIndexesToRemove) {
                    po.OrderForms[0].LineItems.Remove(item);
                }

                po.Save();
            } catch (Exception ex) {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CleanUpChangeOrder: ", ex);

                throw ex;
            }
        }

        private static string GetNextControlNumber() {
            try {
                string controlNumber = string.Empty;
                // get tracking number from DB
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BEKDBContext"].ConnectionString)) {
                    using (SqlCommand cmd = new SqlCommand("Orders.usp_GetNextControlNumber", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter parm = new SqlParameter();
                        parm.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parm);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        controlNumber = (int.Parse(cmd.Parameters[0].Value.ToString())).ToString("0000000.##"); // format to main frame of 7 digits
                    }
                }
                return controlNumber;
            } catch (Exception ex) {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetNextControlNumber: ", ex);

                throw ex;
            }
        }

        private static PurchaseOrder GetPurchaseOrder(Guid userId, Guid cartId) {
            try {
                CommerceServer.Core.Runtime.Orders.OrderContext context = Extensions.SiteHelper.GetOrderContext();
                return context.GetPurchaseOrder(userId, cartId);
            } catch (Exception ex) {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetPurchaseOrder: ", ex);

                throw ex;
            }
        }

        public XmlElement GetUnconfirmatedOrders() {
            var manager = Extensions.SiteHelper.GetOrderManageContext().PurchaseOrderManager;
            System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", "Submitted");
            DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

            int c = results.Tables.Count;

            // Get the value of the OrderGroupId property of each
            // purchase order.
            List<Guid> poIds = new List<Guid>();
            foreach (DataRow row in results.Tables[0].Rows) {
                poIds.Add(new Guid(row["OrderGroupId"].ToString()));
            }
            // Get the XML representation of the purchase orders.
            return manager.GetPurchaseOrdersAsXml(poIds.ToArray());
        }

        public string SaveOrderAsChangeOrder(Guid userId, Guid cartId)
        {
            try
            {
                PurchaseOrder po = GetPurchaseOrder(userId, cartId);

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(og: po, 
                                           transacted: true, 
                                           loggingEnabled: Configuration.EnableLoggingPipeline, 
                                           pipelineName: "Checkout", 
                                           pipelinePath: string.Format
                                               ("{0}\\pipelines\\checkout.pcf", 
                                                HttpContext.Current.Server.MapPath(".")));

                po.TrackingNumber = GetNextControlNumber();
                po.Status = "Submitted";
                po.Save();

                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveOrderAsChangeOrder: ", ex);

                throw ex;
            }
        }

        public string SaveCartAsOrder(Guid userId, Guid cartId)
        {
            try
            {
                var context = Extensions.SiteHelper.GetOrderContext();

                var basket = context.GetBasket(userId, cartId);

                bool reorder = false;
                foreach (LineItem lineItem in basket.OrderForms[0].LineItems)
                {
                    if (lineItem["LinePosition"] == null) reorder = true;
                    else { if (lineItem["LinePosition"].Equals("0")) reorder = true; }
                }
                if (reorder)
                {
                    EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                    eventLog.WriteInformationLog("SaveCartAsOrder : reorder");
                    int startIndex = 1; // main frame needs lineposition to not be null
                    foreach (LineItem lineItem in basket.OrderForms[0].LineItems)
                    {
                        lineItem["LinePosition"] = startIndex;
                        startIndex++;
                    }
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(og: basket, 
                                           transacted: true, 
                                           loggingEnabled: Configuration.EnableLoggingPipeline, 
                                           pipelineName: "Checkout", 
                                           pipelinePath: string.Format
                                               ("{0}\\pipelines\\checkout.pcf", 
                                                HttpContext.Current.Server.MapPath(".")));

                basket.TrackingNumber = GetNextControlNumber();
                basket["OriginalOrderNumber"] = basket.TrackingNumber;
                var purchaseOrder = basket.SaveAsOrder();

                return purchaseOrder.TrackingNumber;
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveCartAsOrder: ", ex);

                throw ex;
            }
        }

        public string UpdatePurchaseOrder(Guid userId, Guid orderId, string requestedShipDate, List<PurchaseOrderLineItemUpdate> lineItemUpdates)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                CommerceServer.Core.Runtime.Orders.LineItem[] lineItems = new CommerceServer.Core.Runtime.Orders.LineItem[po.OrderForms[0].LineItems.Count];
                po.OrderForms[0].LineItems.CopyTo(lineItems, 0);
                po["RequestedShipDate"] = requestedShipDate;

                foreach (PurchaseOrderLineItemUpdate i in lineItemUpdates)
                {
                    CommerceServer.Core.Runtime.Orders.LineItem lineItem = lineItems.Where(x => x.ProductId == i.ItemNumber).FirstOrDefault();
                    // find existing item based on item number
                    if (i.Status == "changed" && lineItem != null)
                    {
                        lineItem.Quantity = i.Quantity;
                        lineItem["Each"] = i.Each;
                        lineItem["CatchWeight"] = i.CatchWeight;
                        lineItem.Status = "changed";
                    }
                    if (i.Status == "deleted" && lineItem != null)
                    {
                        lineItem.Status = "deleted";
                    }
                    if (i.Status == "added" && lineItem == null)
                    {
                        CommerceServer.Core.Runtime.Orders.LineItem li = new CommerceServer.Core.Runtime.Orders.LineItem() { ProductId = i.ItemNumber, Quantity = i.Quantity, Status = "added" };
                        li["CatchWeight"] = i.CatchWeight;
                        li["Each"] = i.Each;
                        li["Notes"] = string.Empty;
                        li.ProductCatalog = i.Catalog;
                        li.Status = "added";
                        li["LinePosition"] = po.OrderForms[0].LineItems.Count + 1;
                        po.OrderForms[0].LineItems.Add(li);
                    }
                    if (i.Status == "added" && lineItem != null)
                    {
                        lineItem.Quantity = i.Quantity;
                        lineItem["Each"] = i.Each;
                        lineItem["CatchWeight"] = i.CatchWeight;
                    }

                }

                if (po.Status.StartsWith("confirmed", StringComparison.InvariantCultureIgnoreCase))
                {
                    po.Status = "Confirmed with un-submitted changes";
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(og: po,
                                           transacted: true,
                                           loggingEnabled: false,
                                           pipelineName: "Checkout",
                                           pipelinePath: string.Format
                                               ("{0}\\pipelines\\checkout.pcf", 
                                                HttpContext.Current.Server.MapPath(".")));

                po.Save();
                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in UpdatePurchaseOrder: ", ex);

                throw ex;
            }
        }

        public void UpdatePurchaseOrderStatus(Guid userId, Guid orderId, string status) {
            try {
                var context = Extensions.SiteHelper.GetOrderContext();
                var po = context.GetPurchaseOrder(userId, orderId);
                //CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                po.Status = status;
                po.Save();
            } catch (Exception ex) {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error Updating Purchase Order Status: ", ex);

                throw;
            }
        }
        #endregion
    }
}
