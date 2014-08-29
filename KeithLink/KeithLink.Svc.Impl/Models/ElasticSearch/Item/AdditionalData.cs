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
        [DataMember(Name="categoryid")]
        public string CategoryId { get; set; }
        
        [DataMember(Name="categoryname")]
        public string CategoryName { get; set; }
        
        [DataMember(Name="categoryname_not_analyzed")]
        public string CategoryNameNotAnalyzed { get; set; }
        
        [DataMember(Name="parentcategoryid")]
        public string ParentCategoryId { get; set; }

        [DataMember(Name="parentcategoryname")]
        public string ParentCategoryName { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="name_not_analyzed")]
        public string NameNotAnalyzed { get; set; }

        [DataMember(Name="description")]
        public string Description { get; set; }

        [DataMember(Name="brand")]
        public string Brand { get; set; }

        [DataMember(Name="brand_not_analyzed")]
        public string BrandNotAnalyzed { get; set; }

        [DataMember(Name="brand_description")]
        public string BrandDescription { get; set; }

        [DataMember(Name="brand_description_not_analyzed")]
        public string BrandDescriptionNotAnalyzed { get; set; }

        [DataMember(Name="pack")]
        public string Pack { get; set; }

        [DataMember(Name="size")]
        public string Size { get; set; }

        [DataMember(Name="upc")]
        public string Upc { get; set; }

        [DataMember(Name="mfrnumber")]
        public string MfrNumber { get; set; }

        [DataMember(Name="mfrname")]
        public string MfrName { get; set; }

        [DataMember(Name="cases")]
        public string Cases { get; set; }

        [DataMember(Name="package")]
        public string Package { get; set; }

        [DataMember(Name="preferreditemcode")]
        public string PreferredItemCode { get; set; }

        [DataMember(Name="itemtype")]
        public string ItemType { get; set; }

        [DataMember(Name="status1")]
        public string Status1 { get; set; }

        [DataMember(Name="status2")]
        public string Status2 { get; set; }

        [DataMember(Name="caseonly")]
        public string CaseOnly { get; set; }

        [DataMember(Name="specialorderitem")]
        public string SpecialOrderItem { get; set; }

        [DataMember(Name="vendor1")]
        public string Vendor1 { get; set; }

        [DataMember(Name="vendor2")]
        public string Vendor2 { get; set; }

        [DataMember(Name="itemclass")]
        public string ItemClass { get; set; }

        [DataMember(Name="catmgr")]
        public string CatMgr { get; set; }

        [DataMember(Name="buyer")]
        public string Buyer { get; set; }

        [DataMember(Name="kosher")]
        public string Kosher { get; set; }

        [DataMember(Name="branchid")]
        public string BranchId { get; set; }

        [DataMember(Name="replacementitem")]
        public string ReplacementItem { get; set; }

        [DataMember(Name="replaceditem")]
        public string ReplacedItem { get; set; }

        [DataMember(Name="childnutrition")]
        public string ChildNutrition { get; set; }

        [DataMember(Name="itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name="nutritional")]
        public NutritionalInformation Nutritional { get; set; }

        [DataMember(Name="nonstock")]
		public string NonStock { get; set; }

        [DataMember(Name="itemspecification")]
		public List<string> ItemSpecification { get; set; }
    }
}
