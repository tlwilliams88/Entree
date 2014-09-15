using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="UserProfile")]
    public class UserProfile : System.Security.Principal.IIdentity
    {
        [DataMember(Name="UserId")]
        public Guid UserId {get;set;}
        [DataMember(Name = "UserName")]
		public string UserName { get; set; }
        [DataMember(Name="FirstName")]
		public string FirstName { get; set; }
        [DataMember(Name="LastName")]
		public string LastName { get; set; }
        [DataMember(Name="EmailAddress")]
		public string EmailAddress { get; set; }
        [DataMember(Name="PhoneNumber")]
		public string PhoneNumber { get; set; }
        [DataMember(Name="CustomerName")]
		public string CustomerName { get; set; }

        public string AuthenticationType
        {
            get { return "Active Directory"; }
        }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "BranchId")]
		public string BranchId { get; set; }

		public bool IsAuthenticated { get; set; }
        public string Name
        {
            get { return EmailAddress; }
        }
    }
}
