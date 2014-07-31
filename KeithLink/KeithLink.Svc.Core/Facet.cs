using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Catalog
{
    [DataContract(Name = "facet")]
    [Serializable]
    public class Facet
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "facetvalues")]
        public List<FacetValue> FacetValues { get; set; }
    }
}
