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
        [DataMember(Name = "cartItems")]
        public List<string> CartItems { get; set; }

        [DataMember(Name = "pagesize")]
        public int? PageSize { get; set; }

        [DataMember(Name = "getimages")]
        public bool? GetImages { get; set; }
    }
}
