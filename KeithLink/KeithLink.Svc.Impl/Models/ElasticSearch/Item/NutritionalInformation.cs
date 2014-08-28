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
        [DataMember(Name="brandowner")]
        public string BrandOwner { get; set; }

        [DataMember(Name="countryoforigin")]
        public string CountryOfOrigin { get; set; }

        [DataMember(Name="grossweight")]
        public string GrossWeight { get; set; }

        [DataMember(Name="handlinginstruction")]
        public string HandlingInstruction { get; set; }

        [DataMember(Name="ingredients")]
        public string Ingredients { get; set; }

        [DataMember(Name="itemidentificationcode")]
        public string ItemIdentificationCode { get; set; }

        [DataMember(Name="marketingmessage")]
        public string MarketingMessage { get; set; }

        [DataMember(Name="moreinformation")]
        public string MoreInformation { get; set; }

        [DataMember(Name="servingsize")]
        public string ServingSize { get; set; }

        [DataMember(Name="servingsizeuom")]
        public string ServingSizeUom { get; set; }

        [DataMember(Name="servingsperpack")]
        public string ServingsPerPack { get; set; }

        [DataMember(Name="servingsuggestion")]
        public string ServingSuggestion { get; set; }

        [DataMember(Name="shelf")]
        public string Shelf { get; set; }

        [DataMember(Name="storagetemp")]
        public string StorageTemp { get; set; }

        [DataMember(Name="unitmeasure")]
        public string UnitMeasure { get; set; }

        [DataMember(Name="unitspercase")]
        public string UnitsPerCase { get; set; }

        [DataMember(Name="volume")]
        public string Volume { get; set; }

        [DataMember(Name="height")]
        public string Height { get; set; }

        [DataMember(Name="length")]
        public string Length { get; set; }

        [DataMember(Name="width")]
        public string Width { get; set; }

        
        [DataMember(Name="nutrition")]
        public List<ItemNutrition> Nutrition { get; set; }

        [DataMember(Name="diet")]
        public List<Diet> Diet { get; set; }

        [DataMember(Name="allergen")]
        public Allergen Allergen { get; set; } 
    }
}
