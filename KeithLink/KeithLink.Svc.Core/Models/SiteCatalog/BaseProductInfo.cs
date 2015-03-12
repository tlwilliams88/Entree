using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
	[DataContract]
    public class BaseProductInfo
	{
		private string pack;
		
		[DataMember(Name = "itemnumber")]
		[Description("Item")]
		public string ItemNumber { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "nonstock", EmitDefaultValue = false)]
		public string NonStock { get; set; }

		[DataMember(Name = "caseprice", EmitDefaultValue = false)]
		[Description("Price")]
		public string CasePrice { get; set; }

        [IgnoreDataMember]
        public double CasePriceNumeric { get; set; }

		[DataMember(Name = "packageprice", EmitDefaultValue = false)]
		public string PackagePrice { get; set; }

		[DataMember(Name = "replacementitem", EmitDefaultValue = false)]
		public string ReplacementItem { get; set; }

		[DataMember(Name = "replaceditem", EmitDefaultValue = false)]
		public string ReplacedItem { get; set; }

		[DataMember(Name = "childnutrition", EmitDefaultValue = false)]
		public string ChildNutrition { get; set; }

		[DataMember(Name = "brand", EmitDefaultValue = false)]
		[Description("Brand")]
		public string Brand { get; set; }

		[DataMember(Name = "brand_extended_description", EmitDefaultValue = false)]
		[Description("Brand")]
        public string BrandExtendedDescription { get; set; }

		[DataMember(Name = "brand_control_label", EmitDefaultValue = false)]
        public string BrandControlLabel { get; set; }

		[DataMember(Name = "name", EmitDefaultValue = false)]
		[Description("Name")]
		public string Name { get; set; }

		[DataMember(Name = "favorite")]
		public bool Favorite { get; set; }

		[DataMember(Name = "notes")]
		[Description("Note")]
		public string Notes { get; set; }

		[DataMember(Name = "catchweight", EmitDefaultValue = false)]
		[Description("Catch Weight")]
        public bool CatchWeight { get; set; }

		[DataMember(Name = "sellsheet", EmitDefaultValue = false)]
        public string SellSheet { get; set; }

		[DataMember(Name = "deviatedcost", EmitDefaultValue = false)]
        public string DeviatedCost { get; set; }

		[DataMember(Name = "temp_zone")]
        public string TempZone { get; set; }

		[DataMember(Name = "categoryId", EmitDefaultValue = false)]
		[Description("Category")]
		public string CategoryId { get; set; }

		[DataMember(Name = "categoryname", EmitDefaultValue = false)]
		[Description("Category Desc")]
		public string CategoryName { get; set; }

		[DataMember(Name = "class", EmitDefaultValue = false)]
		[Description("Class")]
		public string ItemClass { get; set; }

		[DataMember(Name = "vendor_num", EmitDefaultValue = false)]
		[Description("Vendor Item #")]
		public string VendorItemNumber { get; set; }

		[DataMember(Name = "upc", EmitDefaultValue = false)]
		public string UPC { get; set; }

		[DataMember(Name = "size", EmitDefaultValue = false)]
		public string Size { get; set; }

		[DataMember(Name = "pack", EmitDefaultValue = false)]
		public string Pack
		{
			get { return string.IsNullOrEmpty(pack) ? null : pack.TrimStart(new char[] { '0' }); }
			set { pack = value; }
		}

		[DataMember(Name = "packsize")]
		[Description("Pack/Size")]
        public string PackSize { get { return string.Format("{0} / {1}", this.Pack, this.Size); } set { } }

		[DataMember(Name = "cases", EmitDefaultValue = false)]
		[Description("Cases")]
		public string Cases { get; set; }

		[DataMember(Name = "nutritional", EmitDefaultValue = false)]
		public Nutritional Nutritional { get; set; }

		[DataMember(Name = "kosher", EmitDefaultValue = false)]
		public string Kosher { get; set; }

		[DataMember(Name = "manufacturer_number", EmitDefaultValue = false)]
		[Description("Manufacturer Number")]
		public string ManufacturerNumber { get; set; }

		[DataMember(Name = "manufacturer_name", EmitDefaultValue = false)]
		[Description("Manufacturer Name")]
		public string ManufacturerName { get; set; }

		[DataMember(Name = "average_weight", EmitDefaultValue = false)]
        [Description("Average Weight")]
        public double AverageWeight { get; set; }
    }
}
