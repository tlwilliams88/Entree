using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System.ComponentModel;

namespace KeithLink.Svc.Core.Models.Reports
{
    [DataContract(Name = "reportitemusage")]
    public class ItemUsageReportItemModel: BaseProductInfo, IExportableModel
    {
        public ItemUsageReportItemModel() 
        { 
            PackSize = string.Empty; 
        }

        [DataMember(Name = "itemnumber")]
		[Description("Item")]
        public new string ItemNumber { get; set; }

        [DataMember(Name = "detail")]
        [Description("Detail")]
        public string Detail { get; set; }

        [DataMember(Name = "name")]
		[Description("Name")]
        public new string Name { get; set; }

        [DataMember(Name = "class")]
        [Description("Category")]
        public string Class { get; set; }

        [DataMember(Name = "images")]
        public List<ProductImage> Images { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        [DataMember(Name = "totalquantityordered")]
		[Description("Qty Ordered")]
        public int TotalQuantityOrdered { get; set; }

        [DataMember(Name = "totalquantityshipped")]
		[Description("Qty Shipped")]
        public int TotalQuantityShipped { get; set; }

        [DataMember(Name = "packsize")]
		[Description("Pack/Size")]
        public new string PackSize { get; set; }

        public new string Pack { get; set; }
        public new string Size { get; set; }
        [DataMember(Name = "each")]
		[Description("Each")]
        public string Each { get; set; }

		[DataMember(Name = "averageprice")]
		[Description("Average Price")]
		public decimal AveragePrice { get; set; }


		[DataMember(Name = "totalcost")]
		[Description("Total Cost")]
		public decimal TotalCost { get; set; }

		

		public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 5, Label = "Name" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Brand", Order = 10, Label = "Brand" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Class", Order = 15, Label = "Category" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "ManufacturerName", Order = 20, Label = "Mfr Name" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "UPC", Order = 25, Label = "GTIN" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "VendorItemNumber", Order = 30, Label = "Vendor Item" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Pack", Order = 35, Label = "Pack" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Size", Order = 36, Label = "Size" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Each", Order = 40, Label = "Each" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "TotalQuantityOrdered", Order = 45, Label = "Qty Ordered" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "TotalQuantityShipped", Order = 50, Label = "Qty Shipped" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "AveragePrice", Order = 55, Label = "Average Price" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "TotalCost", Order = 60, Label = "Total Cost" });



            return defaultConfig;
		}
	}
}
