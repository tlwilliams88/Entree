using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.BrandControlLabels
{
    public class BrandData
    {
        [DataMember(Name = "brand_control_label")]
        public string BrandControlLabel { get; set; }

        [DataMember(Name = "extended_description")]
        public string ExtendedDescription { get; set; }
    }
}
