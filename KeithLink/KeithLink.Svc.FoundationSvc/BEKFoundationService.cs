
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
			var siteName = ConfigurationManager.AppSettings["CS_Sitename"].ToString();

			var context = CommerceServer.Core.Runtime.Orders.OrderContext.Create(siteName);
			PipelineHelper pipeLineHelper = new PipelineHelper(siteName, context);

			var basket = context.GetBasket(userId, cartId);

			pipeLineHelper.RunPipeline(basket, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));


			var purchaseOrder = basket.SaveAsOrder();

			return purchaseOrder.TrackingNumber;
		}

        public string SaveOrderAsChangeOrder(Guid userId, Guid cartId)
        {

            return string.Empty;
        }
	}

	
}