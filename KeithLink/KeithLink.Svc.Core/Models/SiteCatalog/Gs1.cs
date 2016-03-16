using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "nutrition")]
    [Serializable]
    public class Nutrition
    {
        [DataMember(Name = "dailyvalue")]
        public string DailyValue { get; set; }
        [DataMember(Name = "measurementvalue")]
        public string MeasurementValue { get; set; }
        [DataMember(Name = "measurementtypeid")]
        public string MeasurementTypeId { get; set; }
        [DataMember(Name = "nutrienttypecode")]
        public string NutrientTypeCode { get; set; }
        [DataMember(Name = "nutrienttype")]
        public string NutrientType { get; set; }
    }

    [DataContract(Name = "diet")]
    [Serializable]
    public class Diet
    {
        [DataMember(Name = "diettype")]
        public string DietType { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract(Name = "allergen")]
    [Serializable]
    public class Allergen
    {
		[DataMember(Name = "freefrom")]
		public List<string> freefrom { get; set; }
		[DataMember(Name = "maycontain")]
		public List<string> maycontain { get; set; }
		[DataMember(Name = "contains")]
		public List<string> contains { get; set; }
    }

    [DataContract(Name = "nutritional")]
    [Serializable]
    public class Nutritional
    {
        [DataMember(Name = "brandowner")]
        public string BrandOwner { get; set; }
        [DataMember(Name = "countryoforigin")]
		[Description("Country Of Origin")]
        public string CountryOfOrigin { get; set; }
        [DataMember(Name = "grossweight")]
		[Description("Gross Weight")]
        public string GrossWeight { get; set; }
        [DataMember(Name = "handlinginstruction")]
		[Description("Handling Instructions")]
        public string HandlingInstructions { get; set; }
        [DataMember(Name = "ingredients")]
        public string Ingredients { get; set; }
        [DataMember(Name = "marketingmessage")]
        public string MarketingMessage { get; set; }
        [DataMember(Name = "moreinformation")]
        public string MoreInformation { get; set; }
        [DataMember(Name = "servingsize")]
        public string ServingSize { get; set; }
        [DataMember(Name = "servingsizeuom")]
        public string ServingSizeUOM { get; set; }
        [DataMember(Name = "servingsperpack")]
        public string ServingsPerPack { get; set; }
        [DataMember(Name = "servingsuggestion")]
        public string ServingSugestion { get; set; }
        [DataMember(Name = "shelf")]
        public string Shelf { get; set; }
        [DataMember(Name = "storagetemp")]
        public string StorageTemp { get; set; }
        [DataMember(Name = "unitmeasure")]
        public string UnitMeasure { get; set; }
        [DataMember(Name = "unitspercase")]
        public string UnitsPerCase { get; set; }
        [DataMember(Name = "volume")]
        public string Volume { get; set; }
        [DataMember(Name = "height")]
        public string Height { get; set; }
        [DataMember(Name = "length")]
        public string Length { get; set; }
        [DataMember(Name = "width")]
        public string Width { get; set; }
        [DataMember(Name = "nutrition")]
        public List<Nutrition> NutritionInfo { get; set; }
        [DataMember(Name = "diet")]
        public List<Diet> DietInfo { get; set; }
        [DataMember(Name = "allergens")]
        public Allergen Allergens { get; set; }
    }

    [DataContract(Name = "unfi")]
    [Serializable]
    public class UNFI
    {
        [DataMember(Name = "caseprice")]
        public string CasePrice { get; set; }
        [DataMember(Name = "caselength")]
        public string CaseLength { get; set; }
        [DataMember(Name = "casewidth")]
        public string CaseWidth { get; set; }
        [DataMember(Name = "caseheight")]
        public string CaseHeight { get; set; }
        [DataMember(Name = "packageprice")]
        public string PackagePrice { get; set; }
        [DataMember(Name = "packagelength")]
        public string PackageLength { get; set; }
        [DataMember(Name = "packageheight")]
        public string PackageHeight { get; set; }
        [DataMember(Name = "packagewidth")]
        public string PackageWidth { get; set; }
        [DataMember(Name = "unitofsale")]
        public string UnitOfSale { get; set; }
        [DataMember(Name = "weight")]
        public string Weight { get; set; }
        [DataMember(Name = "catalogdept")]
        public string CatalogDept { get; set; }
        [DataMember(Name = "shipminexpire")]
        public string ShipMinExpire { get; set; }
        [DataMember(Name = "minorder")]
        public string MinOrder { get; set; }
        [DataMember(Name = "casequantity")]
        public string CaseQuantity { get; set; }
        [DataMember(Name = "putup")]
        public string PutUp { get; set; }
        [DataMember(Name = "contunit")]
        public string ContUnit { get; set; }
        [DataMember(Name = "tcscode")]
        public string TCSCode { get; set; }
        [DataMember(Name = "caseupc")]
        public string CaseUPC { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "flag1")]
        public string Flag1 { get; set; }
        [DataMember(Name = "flag2")]
        public string Flag2 { get; set; }
        [DataMember(Name = "flag3")]
        public string Flag3 { get; set; }
        [DataMember(Name = "flag4")]
        public string Flag4 { get; set; }
        [DataMember(Name = "onhandqty")]
        public string OnHandQty { get; set; }
        [DataMember(Name = "vendor")]
        public string Vendor { get; set; }
        [DataMember( Name = "stockedinbranches" )]
        public string StockedInBranches { get; set; }
    }

}
