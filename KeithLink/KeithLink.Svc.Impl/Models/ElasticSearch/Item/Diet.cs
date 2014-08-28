using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class Diet
    {
        [DataMember(Name = "diettype")]
        public string DietType { get; set; }
    }
}
