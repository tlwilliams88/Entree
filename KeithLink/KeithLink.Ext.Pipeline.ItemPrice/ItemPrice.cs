using System.Runtime.InteropServices;
using CommerceServer.Core.Runtime;
using CommerceServer.Core.Interop;
using CommerceServer.Core.Interop.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Ext.Pipeline.ItemPrice.PipelineService;
using System.Configuration;

namespace KeithLink.Ext.Pipeline.ItemPrice
{
	[ComVisible(true)]
	[GuidAttribute("70A269CB-97B5-40D5-A457-10CACEBDEAB3")]
	public class ItemPrice : IPipelineComponent, IPipelineComponentDescription
	{
		// Status codes for pipeline components
		private const Int32 StatusSuccess = 1; // success
		private const Int32 StatusWarning = 2; // warning
		private const Int32 StatusError = 3; // error

		public void EnableDesign(int fEnable){}

		public int Execute(object pdispOrder, object pdispContext, int lFlags)
		{
			IDictionary order = null;
			IDictionary context = null;

			try
			{				
				order = (IDictionary)pdispOrder;
				context = (IDictionary)pdispContext;
				ISimpleList lineItems = (ISimpleList)order["items"];

				var url = ConfigurationManager.AppSettings.Get("PricingServiceUrl");

				if (lineItems != null && lineItems.Count > 0)
				{
					var products = new List<KeithLink.Ext.Pipeline.ItemPrice.PipelineService.product>() { new product() { itemnumber = "523501" } };

					var prices = PipelineServiceHelper.CreateWebServiceInstance(url).GetPrices("fam", //TODO: Use real branch: order["BranchId"].ToString(),
						"011807", //TODO: Remove hardcoded customerId
						DateTime.Now.AddDays(1), //TODO: Store and grab shipping date from Order
						lineItems.Cast<IDictionary>().Select(p => new product() { itemnumber = p["product_id"].ToString() }).ToArray());
					
					foreach (object lineItem in lineItems)
					{
						IDictionary Item = (IDictionary)lineItem;
						var itemPrice = ((KeithLink.Ext.Pipeline.ItemPrice.PipelineService.PriceReturn)prices).Prices.Where(p => p.ItemNumber.Equals(Item["product_id"].ToString()));

						if (itemPrice.Any())
						{
							var price = Item["Each"].ToString().ToLower() == "true" ? itemPrice.First().PackagePrice : itemPrice.First().CasePrice;

							//if(price == 0) //TODO: Enable this check once we are using a real customer. For now there are far too many products without a price.
							//	throw new Exception("Price Not Found");

							Item["_cy_iadjust_regularprice"] = (decimal)price;

							Item["cy_placed_price"] = (decimal)price;
						}
						else
							throw new Exception("Price Not Found");
					}
				}

				//Taxes? Shipping Cost?
				
				
				order["_cy_oadjust_subtotal"] = lineItems.Cast<IDictionary>().Sum(l => (int)l["quantity"] * (decimal)l["cy_placed_price"]);
				order["_cy_total_total"] = lineItems.Cast<IDictionary>().Sum(l => (int)l["quantity"] * (decimal)l["cy_placed_price"]);

				return StatusSuccess;
			}
			catch (Exception ex)
			{
				Object errorMsg = ex.Message;
				((ISimpleList)order["_Basket_Errors"]).Add(ref errorMsg);
				return StatusError;
			}
		}
		
		public dynamic ContextValuesRead()
		{
			object[] contextValuesRead = new object[0];
			return contextValuesRead;
		}

		public dynamic ValuesRead()
		{
			object[] contextValuesRead = new object[0];
			return contextValuesRead;
		}

		public dynamic ValuesWritten()
		{
			object[] valuesRead = new object[3];
			valuesRead[0] = "_cy_iadjust_regularprices";
			valuesRead[0] = "_cy_oadjust_subtotal";
			valuesRead[0] = "_cy_total_total";
			return valuesRead;
		}

		
	}
}
