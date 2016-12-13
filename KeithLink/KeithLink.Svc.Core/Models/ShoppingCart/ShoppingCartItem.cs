using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract(Name="ShoppingCartItem")]
	public class ShoppingCartItem: BaseProductInfo, IExportableModel 
    {
		[DataMember(Name = "cartitemid")]
		public Guid CartItemId { get; set; }

		[DataMember(Name = "quantity")]
		public decimal Quantity { get; set; }

        public string strPosition { get; set; }
        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "packsize")]
		public new string PackSize { get; set; }

        [DataMember(Name = "name")]
		public new string Name { get; set; }

		[DataMember(Name ="notes")]
		public new string Notes { get; set; }

        [DataMember( Name = "label" )]
        public string Label { get; set; }

        [DataMember( Name = "iscombinedquantity" )]
        public bool IsCombinedQuantity { get; set; }

        [DataMember( Name = "parlevel" )]
        public decimal ParLevel { get; set; }

		[DataMember(Name="each")]
		public bool Each { get; set; }

		[DataMember(Name = "storagetemp")]
		public string StorageTemp { get; set; }

		[DataMember(Name = "createddate")]
		public DateTime CreatedDate { get; set; }
		
		public double LineTotal(double Price)
		{

			if (this.CatchWeight)
			{
				if (this.Each) //package catchweight
				{
					return ((this.AverageWeight / Int32.Parse(this.Pack)) * (double)this.Quantity) * Price;
				}
				else //case catchweight
				{
					return (this.AverageWeight * (double)this.Quantity) * Price;
				}

			}
			else
			{
				return (double)this.Quantity * Price;
			}
		}
        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item #" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 10, Label = "Name" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Brand", Order = 20, Label = "Brand" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemClass", Order = 30, Label = "Class" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Notes", Order = 40, Label = "Notes" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Pack", Order = 50, Label = "Pack" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Size", Order = 60, Label = "Size" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Quantity", Order = 70, Label = "Qty" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Each", Order = 80, Label = "Each" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "CasePrice", Order = 90, Label = "Price" });

            return defaultConfig;
        }
    }
}
