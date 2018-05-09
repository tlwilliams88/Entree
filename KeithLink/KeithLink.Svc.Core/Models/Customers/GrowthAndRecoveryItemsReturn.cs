using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Dynamic;

using KeithLink.Svc.Core.Models.Customers;

using Newtonsoft.Json;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "growthandrecoveryitems")]
    public class GrowthAndRecoveryItemsReturn
    {
        [DataMember(Name = "count")]
        public int Count => Items.Count; 

        [DataMember(Name = "items")]
        public List<GrowthAndRecoveriesModel> Items { get; set; }
    }
}
