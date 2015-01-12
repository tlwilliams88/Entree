using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class AdditionalData
    {
        [JsonProperty("categoryid")]
        public string CategoryId { get; set; }
        
        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }
        
        [JsonProperty("categoryname_not_analyzed")]
        public string CategoryNameNotAnalyzed { get; set; }
        
        [JsonProperty("parentcategoryid")]
        public string ParentCategoryId { get; set; }

        [JsonProperty("parentcategoryname")]
        public string ParentCategoryName { get; set; }

        [JsonProperty("parentcategoryname_not_analyzed")]
        public string ParentCategoryNameNotAnalyzed { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_not_analyzed")]
        public string NameNotAnalyzed { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("brand_not_analyzed")]
        public string BrandNotAnalyzed { get; set; }

        [JsonProperty("brand_description")]
        public string BrandDescription { get; set; }

        [JsonProperty("brand_description_not_analyzed")]
        public string BrandDescriptionNotAnalyzed { get; set; }

        [JsonProperty("brand_control_label")]
        public string BrandControlLabel { get; set; }

        [JsonProperty("pack")]
        public string Pack { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("upc")]
        public string Upc { get; set; }

        [JsonProperty("mfrnumber")]
        public string MfrNumber { get; set; }

        [JsonProperty("mfrname")]
        public string MfrName { get; set; }

        [JsonProperty("mfrname_not_analyzed")]
        public string MfrNameNotAnalyzed { get; set; }

        [JsonProperty("cases")]
        public string Cases { get; set; }

        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("preferreditemcode")]
        public string PreferredItemCode { get; set; }

        [JsonProperty("itemtype")]
        public string ItemType { get; set; }

        [JsonProperty("status1")]
        public string Status1 { get; set; }

        [JsonProperty("status1_not_analyzed")]
        public string Status1NotAnalyzed { get; set; }

        [JsonProperty("status2")]
        public string Status2 { get; set; }

        [JsonProperty("caseonly")]
        public string CaseOnly { get; set; }

        [JsonProperty("specialorderitem")]
        public string SpecialOrderItem { get; set; }

        [JsonProperty("vendor1")]
        public string Vendor1 { get; set; }

        [JsonProperty("vendor2")]
        public string Vendor2 { get; set; }

        [JsonProperty("itemclass")]
        public string ItemClass { get; set; }

        [JsonProperty("catmgr")]
        public string CatMgr { get; set; }

        [JsonProperty("buyer")]
        public string Buyer { get; set; }

        [JsonProperty("kosher")]
        public string Kosher { get; set; }

        [JsonProperty("branchid")]
        public string BranchId { get; set; }

        [JsonProperty("replacementitem")]
        public string ReplacementItem { get; set; }

        [JsonProperty("replaceditem")]
        public string ReplacedItem { get; set; }

        [JsonProperty("childnutrition")]
        public string ChildNutrition { get; set; }

        [JsonProperty("itemnumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("nutritional")]
        public NutritionalInformation Nutritional { get; set; }

        [JsonProperty("nonstock")]
		public string NonStock { get; set; }

        [JsonProperty("itemspecification")]
		public List<string> ItemSpecification { get; set; }

        [JsonProperty("temp_zone")]
        public string TempZone { get; set; }

        [JsonProperty("catchweight")]
        public bool CatchWeight { get; set; }

        [JsonProperty("sellsheet")]
        public string SellSheet { get; set; }

		[JsonProperty("isproprietary")]
		public bool IsProprietary { get; set; }

		[JsonProperty("proprietarycustomers")]
		public string ProprietaryCustomers { get; set; }
    }
}
