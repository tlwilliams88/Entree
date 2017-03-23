using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "externalproduct")]
    [Serializable]
    public class ShallowProduct
    {
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "pack")]
        public string Pack { get; set; }

        [DataMember(Name = "size")]
        public string Size { get; set; }

        [DataMember(Name = "packsize")]
        public string PackSize { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "brand")]
        public string Brand { get; set; }

        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        [DataMember(Name = "upc")]
        public string Upc { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "class")]
        public string Class { get; set; }
    }
}
