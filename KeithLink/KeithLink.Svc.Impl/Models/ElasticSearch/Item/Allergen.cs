using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class Allergen
    {
        public Allergen()
		{
			FreeFrom = new List<string>();
			MayContain = new List<string>();
			Contains = new List<string>();
		}


        [JsonProperty("freefrom")]
		public List<string> FreeFrom { get; set; }

        [JsonProperty("maycontain")]
		public List<string> MayContain { get; set; }

        [JsonProperty("contains")]
        public List<string> Contains { get; set; } 
    }
}
