
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using CommerceServer.Foundation;
using KeithLink.Svc.FoundationSvc.Interface;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Data.SqlClient;
using CommerceServer.Core;

namespace KeithLink.Svc.FoundationSvc
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class BEKFoundationService : OperationService, IBEKFoundationService
	{
        private static string applicationNameForLogging = "KeithLink.FoundationSvc";
		public string SaveCartAsOrder(Guid userId, Guid cartId)
		{
            try
            {
                var context = Extensions.SiteHelper.GetOrderContext();

                var basket = context.GetBasket(userId, cartId);

                int startIndex = 1; // main frame needs these to start at 1
                foreach (CommerceServer.Core.Runtime.Orders.LineItem lineItem in basket.OrderForms[0].LineItems)
                {
                    lineItem["LinePosition"] = startIndex;
                    startIndex++;
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(basket, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                basket.TrackingNumber = GetNextControlNumber();
                basket["OriginalOrderNumber"] = basket.TrackingNumber;
                var purchaseOrder = basket.SaveAsOrder();

                return purchaseOrder.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveCartAsOrder: ", ex);
                    
                throw ex;
            }
		}

        public string SaveOrderAsChangeOrder(Guid userId, Guid cartId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po =
                    GetPurchaseOrder(userId, cartId);

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(po, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                po.TrackingNumber = GetNextControlNumber();
                po.Status = "Submitted";
                po.Save();

                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveOrderAsChangeOrder: ", ex);

                throw ex;
            }
        }

        public void CleanUpChangeOrder(Guid userId, Guid cartId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po =
                    GetPurchaseOrder(userId, cartId);

                List<int> lineItemIndexesToRemove = new List<int>();

                foreach (CommerceServer.Core.Runtime.Orders.LineItem li in po.OrderForms[0].LineItems)
                {
                    if (li.Status == "deleted")
                    {
                        lineItemIndexesToRemove.Add(li.Index);
                    }
                    li.Status = string.Empty;
                }
                foreach (int index in lineItemIndexesToRemove)
                    po.OrderForms[0].LineItems.Remove(index);

                po.Save();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CleanUpChangeOrder: ", ex);

                throw ex;
            }
        }

        private static CommerceServer.Core.Runtime.Orders.PurchaseOrder GetPurchaseOrder(Guid userId, Guid cartId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.OrderContext context = Extensions.SiteHelper.GetOrderContext();
                return context.GetPurchaseOrder(userId, cartId);
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetPurchaseOrder: ", ex);

                throw ex;
            }
        }

        public string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> lineItemUpdates)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                CommerceServer.Core.Runtime.Orders.LineItem[] lineItems = new CommerceServer.Core.Runtime.Orders.LineItem[po.OrderForms[0].LineItems.Count];
                po.OrderForms[0].LineItems.CopyTo(lineItems, 0);
                po["RequestedShipDate"] = requestedShipDate;
                int linePosition = 1; // main frame needs these to start at 1
                foreach (var li in lineItems)
                    if (linePosition < (int)li["LinePosition"])
                        linePosition = (int)li["LinePosition"];
                linePosition++;

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
                        li["LinePosition"] = linePosition;
                        li.ProductCatalog = i.Catalog;
                        linePosition++;
                        po.OrderForms[0].LineItems.Add(li);
                    }
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(po, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                po.Save();
                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in UpdatePurchaseOrder: ", ex);

                throw ex;
            }
        }

        public string CancelPurchaseOrder(Guid userId, Guid orderId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                po.Status = "Cancelled";
                po.TrackingNumber = GetNextControlNumber();
                po.Save();
                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CancelPurchaseOrder: ", ex);

                throw ex;
            }
        }

        private static string GetNextControlNumber()
        {
            try
            {
                string controlNumber = string.Empty;
                // get tracking number from DB
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppDataConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Orders.usp_GetNextControlNumber", conn))
                    {
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
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetNextControlNumber: ", ex);

                throw ex;
            }
        }

		public System.Xml.XmlElement GetUnconfirmatedOrders()
		{
			var manager = Extensions.SiteHelper.GetOrderManageContext().PurchaseOrderManager;
			System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
			SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
			SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", "Submitted");
			DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

			int c = results.Tables.Count;

			// Get the value of the OrderGroupId property of each
			// purchase order.
			List<Guid> poIds = new List<Guid>();
			foreach (DataRow row in results.Tables[0].Rows)
			{
				poIds.Add(new Guid(row["OrderGroupId"].ToString()));
			}
			// Get the XML representation of the purchase orders.
			return manager.GetPurchaseOrdersAsXml(poIds.ToArray());
		}
    }	
}