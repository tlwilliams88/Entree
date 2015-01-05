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
        public Guid Id { get; set; }
        
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "customers")]
        public List<Customer> Customers { get; set; }

        [DataMember(Name = "adminusers")]
        public List<UserProfile> AdminUsers { get; set; }

        [DataMember(Name = "customerusers")]
        public List<UserProfile> CustomerUsers { get; set; }
    }

    public class AccountComparer : EqualityComparer<Account>
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override bool Equals(Account x, Account y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(Account obj)
        {
            return obj.GetHashCode();
        }
    }

    public class AccountAddUserModel
    {

    }
}
