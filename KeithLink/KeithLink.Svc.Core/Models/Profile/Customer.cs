using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class Customer
    {
        [DataMember(Name = "customerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "customerName")]
        public string CustomerName { get; set; }

        [DataMember(Name = "customerBranch")]
        public string CustomerBranch { get; set; }
    }
}
