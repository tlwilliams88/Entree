
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

			pipeLineHelper.RunPipeline(basket, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

            basket.TrackingNumber = GetNextControlNumber();
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

        private static CommerceServer.Core.Runtime.Orders.PurchaseOrder GetPurchaseOrder(Guid userId, Guid cartId)
        {
            CommerceServer.Core.Runtime.Orders.OrderContext context = Extensions.SiteHelper.GetOrderContext();
            return context.GetPurchaseOrder(userId, cartId);
        }


        public string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> lineItemUpdates)
        {
            CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
            // make all updates
            po.TrackingNumber = GetNextControlNumber();

            foreach (PurchaseOrderLineItemUpdate i in lineItemUpdates)
            {
                // find existing item based on item number

            }

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
                    controlNumber = cmd.Parameters[0].Value.ToString();
                }
            }
            return controlNumber;
        }
    }

	
}