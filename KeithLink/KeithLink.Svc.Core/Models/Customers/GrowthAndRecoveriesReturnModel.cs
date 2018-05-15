using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Dynamic;

using KeithLink.Svc.Core.Models.Customers;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "growthandrecovery")]
    public class GrowthAndRecoveriesReturnModel
    {
        [DataMember(Name = "code")]
        public string GroupingCode { get; set; }

        [DataMember(Name = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GrowthAndRecoveryType GrowthAndReccoveryTypeKey { get; set; }

        [DataMember(Name = "image")]
        public CategoryImageReturn Image { get; set; }
    }
}
