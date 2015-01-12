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
    public class ItemUsageReportItemModel: IExportableModel
    {
        public ItemUsageReportItemModel() 
        { 
            PackSize = string.Empty; 
        }

        [DataMember(Name = "itemnumber")]
		[Description("Item")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "name")]
		[Description("Name")]
        public string Name { get; set; }
        
        [DataMember(Name = "images")]
        public List<ProductImage> Images { get; set; }

        [DataMember(Name = "totalquantityordered")]
		[Description("Qty Ordered")]
        public int TotalQuantityOrdered { get; set; }

        [DataMember(Name = "totalquantityshipped")]
		[Description("Qty Shipped")]
        public int TotalQuantityShipped { get; set; }

        [DataMember(Name = "packsize")]
		[Description("Pack/Size")]
        public string PackSize { get; set; }

        [DataMember(Name = "each")]
		[Description("Each")]
        public string Each { get; set; }

		public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 20, Label = "Name" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "TotalQuantityOrdered", Order = 30, Label = "Qty Ordered" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "TotalQuantityShipped", Order = 40, Label = "Qty Shipped" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 50, Label = "Pack / Size" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Each", Order = 60, Label = "Each" });

			return defaultConfig;
		}
	}
}
