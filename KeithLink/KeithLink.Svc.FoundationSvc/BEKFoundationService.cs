
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

namespace KeithLink.Svc.FoundationSvc
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class BEKFoundationService : OperationService, IBEKFoundationService
	{
		public string SaveCartAsOrder(Guid userId, Guid cartId)
		{
            var context = Extensions.SiteHelper.GetOrderContext();
			PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());

			var basket = context.GetBasket(userId, cartId);

            int startIndex = 1; // main frame needs these to start at 1
            foreach (CommerceServer.Core.Runtime.Orders.LineItem  lineItem in basket.OrderForms[0].LineItems)
            {
                lineItem["LinePosition"] = startIndex;
                startIndex++;
            }
			pipeLineHelper.RunPipeline(basket, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

            basket.TrackingNumber = GetNextControlNumber();
            basket["OriginalOrderNumber"] = basket.TrackingNumber;
			var purchaseOrder = basket.SaveAsOrder();

			return purchaseOrder.TrackingNumber;
		}

        public string SaveOrderAsChangeOrder(Guid userId, Guid cartId)
        {
            CommerceServer.Core.Runtime.Orders.PurchaseOrder po =
                GetPurchaseOrder(userId, cartId);

            PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
            pipeLineHelper.RunPipeline(po, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

            po.TrackingNumber = GetNextControlNumber();
            po.Save();

            return po.TrackingNumber;
        }

        public void CleanUpChangeOrder(Guid userId, Guid cartId)
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

        private static CommerceServer.Core.Runtime.Orders.PurchaseOrder GetPurchaseOrder(Guid userId, Guid cartId)
        {
            CommerceServer.Core.Runtime.Orders.OrderContext context = Extensions.SiteHelper.GetOrderContext();
            return context.GetPurchaseOrder(userId, cartId);
        }

        public string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> lineItemUpdates)
        {
            CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
            CommerceServer.Core.Runtime.Orders.LineItem[] lineItems = new CommerceServer.Core.Runtime.Orders.LineItem[po.OrderForms[0].LineItems.Count];
            po.OrderForms[0].LineItems.CopyTo(lineItems, 0);
            int linePosition = 1; // main frame needs these to start at 1
            foreach (var li in lineItems)
                if (linePosition < (int.Parse((string)li["LinePosition"])))
                    linePosition = (int.Parse((string)li["LinePosition"]));
            linePosition++;

            foreach (PurchaseOrderLineItemUpdate i in lineItemUpdates)
            {
                CommerceServer.Core.Runtime.Orders.LineItem lineItem = lineItems.Where(x => x.ProductId == i.ItemNumber).FirstOrDefault();
                // find existing item based on item number
                if (i.Status == "changed" && lineItem != null)
                {
                    lineItem.Quantity = i.Quantity;
                    lineItem.Status = "changed";
                }
                if (i.Status == "deleted" && lineItem != null)
                {
                    lineItem.Status = "deleted";
                }
                if (i.Status == "added" && lineItem == null)
                {
                    CommerceServer.Core.Runtime.Orders.LineItem li = new CommerceServer.Core.Runtime.Orders.LineItem() { ProductId = i.ItemNumber, Quantity = i.Quantity, Status = "added" };
                    li["CatchWeight"] = false;
                    li["Each"] = false;
                    li["Notes"] = string.Empty;
                    li["LinePosition"] = linePosition;
                    li.ProductCatalog = i.Catalog; // todo, wire up catalog...
                    linePosition++;
                    po.OrderForms[0].LineItems.Add(li);
                }
            }
            po.Save();
            return po.TrackingNumber;
        }

        private static string GetNextControlNumber()
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
    }	
}