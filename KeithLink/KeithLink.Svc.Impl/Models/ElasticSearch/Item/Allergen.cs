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


        [DataMember(Name="freefrom")]
		public List<string> FreeFrom { get; set; }

        [DataMember(Name="maycontain")]
		public List<string> MayContain { get; set; }

        [DataMember(Name="contains")]
        public List<string> Contains { get; set; } 
    }
}
