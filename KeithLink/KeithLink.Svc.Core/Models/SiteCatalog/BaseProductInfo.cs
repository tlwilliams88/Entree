using System;
using System.Collections.Generic;
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
		public string ItemNumber { get; set; }

		[DataMember(Name = "nonstock")]
		public string NonStock { get; set; }

		[DataMember(Name = "caseprice")]
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
		public string Brand { get; set; }

        [DataMember(Name = "brand_extended_description")]
        public string BrandExtendedDescription { get; set; }

        [DataMember(Name = "brand_control_label")]
        public string BrandControlLabel { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "favorite")]
		public bool Favorite { get; set; }

		[DataMember(Name = "notes")]
		public string Notes { get; set; }

        [DataMember(Name = "catchweight")]
        public bool CatchWeight { get; set; }

        [DataMember(Name = "sellsheet")]
        public string SellSheet { get; set; }

        [DataMember(Name="deviatedcost")]
        public string DeviatedCost { get; set; }
	}
}
