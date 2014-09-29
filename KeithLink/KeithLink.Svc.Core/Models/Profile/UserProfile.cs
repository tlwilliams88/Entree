using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="userprofile")]
    public class UserProfile : System.Security.Principal.IIdentity
    {
        
        public string AuthenticationType
        {
            get { return "Active Directory"; }
        }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        //[DataMember(Name="customername")]
        //public string CustomerName { get; set; }

        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "emailaddress")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }

        public bool IsAuthenticated { get; set; }

        [DataMember(Name = "lastname")]
        public string LastName { get; set; }

        public string Name {
            get { return EmailAddress; }
        }

        [DataMember(Name = "phonenumber")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "rolename")]
        public string RoleName { get; set; }

        [DataMember(Name = "userid")]
        public Guid UserId {get;set;}
        
        //[DataMember(Name = "username")]
        //public string UserName { get; set; }

        [DataMember(Name = "user_customers")]
        public List<Customer> UserCustomers { get; set; }
    }
}
