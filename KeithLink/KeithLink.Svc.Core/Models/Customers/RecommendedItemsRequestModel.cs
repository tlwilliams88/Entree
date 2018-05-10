using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    [DataContract]
    public class RecommendedItemsRequestModel
    {
        [DataMember(Name = "itemnumbers")]
        public List<string> itemnumbers { get; set; }

        [DataMember(Name = "pagesize")]
        public int? pagesize { get; set; }

        [DataMember(Name = "hasimages")]
        public bool? hasimages { get; set; }
    }
}
