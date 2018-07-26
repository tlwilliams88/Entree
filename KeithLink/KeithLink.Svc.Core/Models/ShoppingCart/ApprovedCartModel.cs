// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart {
    [DataContract]
    public class ApprovedCartModel
    {

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "approvedamount")]
        public decimal ApprovedAmount { get; set; }

        [DataMember(Name = "remainingamount")]
        public decimal RemainingAmount { get; set; }

        [DataMember(Name = "approvedordenied")]
        public bool ApprovedOrDenied { get; set; }
    }


}
