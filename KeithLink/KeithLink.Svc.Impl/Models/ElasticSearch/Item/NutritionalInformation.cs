using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class NutritionalInformation
    {
        [JsonProperty("brandowner")]
        public string BrandOwner { get; set; }

        [JsonProperty("countryoforigin")]
        public string CountryOfOrigin { get; set; }

        [JsonProperty("grossweight")]
        public string GrossWeight { get; set; }

        [JsonProperty("handlinginstruction")]
        public string HandlingInstruction { get; set; }

        [JsonProperty("ingredients")]
        public string Ingredients { get; set; }

        [JsonProperty("itemidentificationcode")]
        public string ItemIdentificationCode { get; set; }

        [JsonProperty("marketingmessage")]
        public string MarketingMessage { get; set; }

        [JsonProperty("moreinformation")]
        public string MoreInformation { get; set; }

        [JsonProperty("servingsize")]
        public string ServingSize { get; set; }

        [JsonProperty("servingsizeuom")]
        public string ServingSizeUom { get; set; }

        [JsonProperty("servingsperpack")]
        public string ServingsPerPack { get; set; }

        [JsonProperty("servingsuggestion")]
        public string ServingSuggestion { get; set; }

        [JsonProperty("shelf")]
        public string Shelf { get; set; }

        [JsonProperty("storagetemp")]
        public string StorageTemp { get; set; }

        [JsonProperty("unitmeasure")]
        public string UnitMeasure { get; set; }

        [JsonProperty("unitspercase")]
        public string UnitsPerCase { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        
        [JsonProperty("nutrition")]
        public List<ItemNutrition> Nutrition { get; set; }

        [JsonProperty("diet")]
        public List<Diet> Diet { get; set; }

        [JsonProperty("allergen")]
        public Allergen Allergen { get; set; } 
    }
}
