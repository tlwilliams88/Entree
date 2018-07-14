using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.BrandControlLabels
{
    public class RootData
    {
        public string _index { get { return "brands"; } }
        public string _type { get { return "brand"; } }
        public string _id { get; set; }

        [JsonIgnore]
        public BrandData data { get; set; }
    }
}
