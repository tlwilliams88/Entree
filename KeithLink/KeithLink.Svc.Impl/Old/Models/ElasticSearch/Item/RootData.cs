using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class RootData
    {
#region " attributes "
        public string _index { get; set; }
        public string _type { get { return "product"; } }
        public string _id { get; set; }
#endregion

#region " properties "
        [JsonIgnore]
        public AdditionalData data { get; set; }
#endregion
    }
}
