using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class ItemNutrition
    {
        [JsonProperty("dailyvalue")]
        public string DailyValue { get; set; }

        [JsonProperty("measurementvalue")]
        public string MeasurementValue { get; set; }

        [JsonProperty("measurementtypeid")]
        public string MeasurementTypeId { get; set; }

        [JsonProperty("nutrienttypecode")]
        public string NutrientTypeCode { get; set; }

        [JsonProperty("nutrienttype")]
        public string NutrientType { get; set; }
    }
}
