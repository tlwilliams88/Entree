using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models
{
    public class ElasticSearchItemUpdate
    {
        public RootData index { get; set; }


        public string ToJson()
        {
            return string.Format("{0}\n{1}\n", JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(this.index.data));
        }

    }
    public class RootData
    {
        public string _index { get; set; }
        public string _type { get { return "product"; } }
        public string _id { get; set; }
        [JsonIgnore]
        public AdditionalData data { get; set; }
    }

    public class AdditionalData
    {
        public string categoryid { get; set; }
        public string categoryname { get; set; }
        public string parentcategoryid { get; set; }
        public string parentcategoryname { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public string pack { get; set; }
        public string size { get; set; }
        public string upc { get; set; }
        public string mfrnumber { get; set; }
        public string mfrname { get; set; }
        public string cases { get; set; }
        public string package { get; set; }
        public string perferreditemcode { get; set; }
        public string itemtype { get; set; }
        public string status1 { get; set; }
        public string status2 { get; set; }
        public string icseonly { get; set; }
        public string specialorderitem { get; set; }
        public string vendor1 { get; set; }
        public string vendor2 { get; set; }
        public string itemclass { get; set; }
        public string catmgr { get; set; }
        public string buyer { get; set; }
        public string kosher { get; set; }
        public string branchid { get; set; }
        public string replacementitem { get; set; }
        public string replaceditem { get; set; }
        public string cndoc { get; set; }
        public string itemnumber { get; set; }
        public GS1Data gs1 { get; set; }
    }

    public class GS1Data
    {
        public string brandowner { get; set; }
        public string countryoforiginname { get; set; }
        public string countryoforigin { get; set; }
        public string grossweight { get; set; }
        public string handlinginstruction { get; set; }
        public string ingredients { get; set; }
        public string manufactureritemnumber { get; set; }
        public string marketingmessage { get; set; }
        public string moreinformation { get; set; }
        public string servingsize { get; set; }
        public string servingsizeuom { get; set; }
        public string servingsperpack { get; set; }
        public string servingsuggestion { get; set; }
        public string shelf { get; set; }
        public string storagetemp { get; set; }
        public string unitmeasure { get; set; }
        public string unitmeasureuom { get; set; }
        public string unitspercase { get; set; }
        public string volume { get; set; }
        public string height { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        
        
        public List<ItemNutrition> nutrition { get; set; }
        public List<Diet> diet { get; set; }
        public List<Allergen> allergen { get; set; }
    }

    public class ItemNutrition
    {
        public string dailyvalue { get; set; }
        public string measurementvalue { get; set; }
        public string measurementtypeid { get; set; }
        public string nutrienttypecode { get; set; }
        public string nutrienttype { get; set; }
    }

    public class Diet
    {
        public string diettype { get; set; }
        public string value { get; set; }
    }

    public class Allergen
    {
        public string allergentype { get; set; }
        public string level { get; set; }
    }

}
