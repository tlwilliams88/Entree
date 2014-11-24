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

		[DataMember(Name = "itemnumber")]
		[Description("Item")]
		public string ItemNumber { get; set; }

		[DataMember(Name = "nonstock")]
		public string NonStock { get; set; }

		[DataMember(Name = "caseprice")]
		[Description("Price")]
		public string CasePrice { get; set; }

        [IgnoreDataMember]
        public double CasePriceNumeric { get; set; }

		[DataMember(Name = "packageprice")]
		public string PackagePrice { get; set; }

		[DataMember(Name = "replacementitem")]
		public string ReplacementItem { get; set; }

		[DataMember(Name = "replaceditem")]
		public string ReplacedItem { get; set; }

		[DataMember(Name = "childnutrition")]
		public string ChildNutrition { get; set; }

		[DataMember(Name = "brand")]
		[Description("Brand")]
		public string Brand { get; set; }

        [DataMember(Name = "brand_extended_description")]
		[Description("Brand")]
        public string BrandExtendedDescription { get; set; }

        [DataMember(Name = "brand_control_label")]
        public string BrandControlLabel { get; set; }

		[DataMember(Name = "name")]
		[Description("Name")]
		public string Name { get; set; }

		[DataMember(Name = "favorite")]
		public bool Favorite { get; set; }

		[DataMember(Name = "notes")]
		[Description("Note")]
		public string Notes { get; set; }

        [DataMember(Name = "catchweight")]
        public bool CatchWeight { get; set; }

        [DataMember(Name = "sellsheet")]
        public string SellSheet { get; set; }

        [DataMember(Name="deviatedcost")]
        public string DeviatedCost { get; set; }

        [DataMember(Name = "temp_zone")]
        public string TempZone { get; set; }

		[DataMember(Name = "categoryId")]
		[Description("Category")]
		public string CategoryId { get; set; }

		[DataMember(Name = "categoryname")]
		[Description("Category Desc")]
		public string CategoryName { get; set; }

		[DataMember(Name = "class")]
		[Description("Class")]
		public string ItemClass { get; set; }

		[DataMember(Name = "vendor_num")]
		[Description("Vendor Item #")]
		public string VendorItemNumber { get; set; }

		[DataMember(Name = "upc")]
		public string UPC { get; set; }
    }
}
