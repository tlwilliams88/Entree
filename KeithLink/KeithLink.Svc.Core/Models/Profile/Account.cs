using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="account")]
    public class Account
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "nationalOrRegionlAcctNumber")]
        public string NationalOrRegionalAccountNumber { get; set; }

        [DataMember(Name = "customers")]
        public List<Customer> customers { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "accountAdmin")]
        public UserProfile AccountAdmin { get; set; }
    }
}
