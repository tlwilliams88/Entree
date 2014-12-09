using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "InvoiceItem")]
    public class InvoiceItemModel:BaseProductInfo, IExportableModel
    {
		[DataMember(Name="id")]
		public long Id { get; set; }
		[DataMember(Name = "linenumber")]
		[Description("Line #")]
		public string LineNumber { get; set; }
		[DataMember(Name="itemnumber")]
		[Description("Item #")]
		public string ItemNumber { get; set; }
		[DataMember(Name = "quantityordered")]
		[Description("# Ordereed")]
		public int? QuantityOrdered { get; set; }
		[DataMember(Name = "quantityshipped")]
		[Description("# Shipped")]
		public int? QuantityShipped { get; set; }
		[DataMember(Name = "catchweight")]
		public bool CatchWeightCode { get; set; }
		[DataMember(Name = "extcatchweight")]
		public decimal? ExtCatchWeight { get; set; }
		[DataMember(Name = "itemprice")]
		[Description("Price")]
		public decimal? ItemPrice { get; set; }
		[DataMember(Name = "salesnet")]
		[Description("Ext Price")]
		public decimal? ExtSalesNet { get; set; }
		[DataMember(Name = "classcode")]
		public string ClassCode { get; set; }
		[DataMember(Name = "packsize")]
		[Description("Pack/Size")]
		public string PackSize { get; set; }

		
		public string InvoiceNumber { get; set; }


		public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "LineNumber", Order = 1 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 10 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Notes", Order = 20 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 30 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Order = 40 });

			defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 50 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Order = 60 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "QuantityShipped", Order = 70 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Each", Order = 80 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemPrice", Order = 90 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "ExtSalesNet", Order = 100 });

			return defaultConfig;
		}
	}
}
