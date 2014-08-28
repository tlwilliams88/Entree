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
        [DataMember(Name="dailyvalue")]
        public string DailyValue { get; set; }

        [DataMember(Name="measurementvalue")]
        public string MeasurementValue { get; set; }

        [DataMember(Name="measurementtypeid")]
        public string MeasurementTypeId { get; set; }

        [DataMember(Name="nutrienttypecode")]
        public string NutrientTypeCode { get; set; }

        [DataMember(Name="nutrienttype")]
        public string NutrientType { get; set; }
    }
}
