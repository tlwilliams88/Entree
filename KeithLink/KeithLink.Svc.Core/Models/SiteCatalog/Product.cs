using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System.ComponentModel;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product: BaseProductInfo, IExportableModel
    {
		private string pack;

        #region ctor
        public Product()
        {
            ProductImages = new List<ProductImage>();
            OrderHistory = new Dictionary<string,int>();
        }
        #endregion

        #region properties
        
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "ext_description")]
        public string ExtendedDescription { get; set; }
		       
        [DataMember(Name = "size")]
        public string Size { get; set; }

        

        [DataMember(Name = "manufacturer_number")]
        public string ManufacturerNumber { get; set; }

        [DataMember(Name = "manufacturer_name")]
        public string ManufacturerName { get; set; }

        [DataMember(Name = "cases")]
        public string Cases { get; set; }

        [DataMember(Name = "kosher")]
        public string Kosher { get; set; }
		                        
        [DataMember(Name = "cube")]
        public string CaseCube { get; set; }

		[DataMember(Name = "pack")]
		public string Pack
		{
			get { return pack.TrimStart(new char[] { '0' }); }
			set { pack = value; }
		}

		[DataMember(Name = "packsize")]
		[Description("Pack/Size")]
		public string PackSize { get { return string.Format("{0} / {1}", this.Pack, this.Size); } }

		[DataMember(Name = "nutritional")]
        public Nutritional Nutritional { get; set; }
        
        [DataMember(Name = "productimages")]
        public List<ProductImage> ProductImages { get; set; }

		[DataMember(Name = "isproprietary")]
		public bool IsProprietary { get; set; }

        [DataMember( Name = "orderhistory" )]
        public Dictionary<string, int> OrderHistory { get; set; }
        #endregion
        
		[OnSerializing]
		void OnSerializing(StreamingContext context)
		{
			//Do not output a "blank" upc
			if (this.UPC.Equals("00000000000000"))
				this.UPC = string.Empty;
		}

		public List<ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Order = 10, Label = "Brand" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 20, Label = "Name" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 30, Label = "Pack/Size" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "CasePrice", Order = 40, Label = "Price" });


			return defaultConfig;
		}
	}

	
}
