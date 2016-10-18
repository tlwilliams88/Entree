using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile.EF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="userprofile")]
	[Serializable]
    public class UserProfile : System.Security.Principal.IIdentity {
        #region ctor
        public UserProfile() {
            DsrAliases = new List<DsrAliasModel>();
        }
        #endregion

        public string AuthenticationType
        {
            get { return "Active Directory"; }
        }

	
        [DataMember(Name = "lastlogin")]	
        public DateTime? LastLogin { get; set; }	
	
        [DataMember(Name = "lastactivity")]
        public DateTime? LastActivity { get; set; }
        	
        [DataMember(Name="passwordexpired")]
        public bool PasswordExpired { get; set; }

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

		public bool IsDSR { get { return RoleName.Equals(Constants.ROLE_NAME_DSR, StringComparison.CurrentCultureIgnoreCase); } }
        public bool IsDSM { get { return RoleName.Equals(Constants.ROLE_NAME_DSM, StringComparison.CurrentCultureIgnoreCase); } }

        [DataMember( Name = "imageurl" )]
        public string ImageUrl { get; set; }

		public string DSRNumber { get; set; }
        public string DSMNumber { get; set; }
		public string DSMRole { get; set; }
		[DataMember(Name = "internal")]
		public bool IsInternalUser { get; set; }
        
        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "usernametoken")]
        public string UserNameToken { get; set; }
        
		[DataMember(Name = "defaultcustomer")]
		public Customer DefaultCustomer { get; set; }

        [DataMember(Name="iskbitcustomer")]
        public bool IsKBITCustomer { get; set; }

        [DataMember( Name = "ispowermenucustomer" )]
        public bool IsPowerMenuCustomer { get; set; }

        [DataMember( Name = "powermenupermissionslink" )]
        public string PowerMenuPermissionsUrl { get; set; }

        [DataMember( Name = "powermenugroupsetupurl" )]
        public string PowerMenuGroupSetupUrl { get; set; }

        [DataMember( Name = "powermenuloginurl" )]
        public string PowerMenuLoginUrl { get; set; }

		[DataMember(Name = "isdemo", EmitDefaultValue = false)]
		public bool? IsDemo { get; set; }

        [DataMember(Name = "dsraliases")]
        public List<DsrAliasModel> DsrAliases { get; set; }

		[DataMember(Name = "canmessage")]
		public bool CanMessage { get; set; }

        /*
         * To add new permissions you must:
         * 1. Add the constant (a generalized value for the permission which is not environment specific),
         * 2. Add a configuration for the expected value (value is specific for this environment),
         * 3. The profile permissions model,
         * 4. Add to the pack and unpack methods (in UserProfileLogicImpl.cs) a conversion between the object for the frontend and the list array used in the backend
         * 5. Add the direct conversion to the ConvertPermission method in the ExternalUserDomainRepository
         */
        [JsonIgnore]
        public List<string> Permissions { get; set; }
        [DataMember(Name = "permit")]
        public UserPermissionsModel Permit { get; set; }

    }
}
