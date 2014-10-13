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
        
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "accountAdmin")]
        public UserProfile AccountAdmin { get; set; }
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

    public class AccountAddCustomerModel
    {
        public Guid accountId { get; set; }
        public Guid customerId { get; set; }
        public string role { get; set; }
    }

    public class AccountAddUserModel
    {
        public Guid accountId { get; set; }
        public Guid userId { get; set; }
        public string role { get; set; }
    }
}
