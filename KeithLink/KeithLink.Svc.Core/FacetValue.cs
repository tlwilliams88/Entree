using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Catalog
{
    [DataContract(Name = "facetvalue")]
    [Serializable]
    public class FacetValue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
