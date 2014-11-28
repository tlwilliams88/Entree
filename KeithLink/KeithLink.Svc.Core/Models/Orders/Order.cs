using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract(Name = "Order")]
	public class Order: IExportableModel
	{	
		[DataMember(Name = "ordernumber")]
		[Description("Order #")]
		public string OrderNumber { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

		[DataMember(Name = "deliverydate")]
		[Description("Delivery Date")]
		public DateTime? DeliveryDate { get; set; }
						
		[DataMember(Name = "invoicenumber")]
		[Description("Invoice #")]
		public string InvoiceNumber { get; set; }

		[DataMember(Name = "invoicestatus")]
		[Description("Invoice Status")]
		public string InvoiceStatus { get; set; }
				
		[DataMember(Name = "itemcount")]
		[Description("Item Count")]
		public int ItemCount { get { return this.Items != null ? this.Items.Sum(l => l.Quantity) : 0; } } // to do: allow setting item count for order history?

		[DataMember(Name = "ordertotal")]
		[Description("Total")]
		public double OrderTotal { get; set; }

		[DataMember(Name = "createddate")]
		[Description("Order Date")]
		public DateTime CreatedDate { get; set; }

        [DataMember(Name = "requestedshipdate")]
        public DateTime RequestedShipDate { get; set; }
        
        [DataMember(Name = "ischangeorderallowed")]
        public bool IsChangeOrderAllowed { get; set; }

		[DataMember(Name ="items")]
		public List<OrderLine> Items { get; set; }

        [DataMember(Name = "commerceid")]
        public Guid CommerceId { get; set; }

		public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "OrderNumber", Order = 1, Label = "Order #" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "CreatedDate", Order = 10, Label = "Order Date" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Status", Order = 20, Label = "Status" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "DeliveryDate", Order = 30, Label = "Delivery Date" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemCount", Order = 40, Label = "Item Count" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "OrderTotal", Order = 50, Label = "Total" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Order = 60, Label = "Invoice #" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceStatus", Order = 70, Label = "Invoice Status" });


			return defaultConfig;
		}
	}
}
