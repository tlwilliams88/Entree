// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    [DataContract]
    public class MinimumOrderAmountModel
    {

        [DataMember(Name = "approvedamount")]
        public decimal ApprovedAmount { get; set; }

    }
}
