using KeithLink.Svc.Core.Interface.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile
{
	public class DemoExternalUserDomainRepositoryImpl : ICustomerDomainRepository
	{
		public Core.Models.Authentication.AuthenticationModel AuthenticateUser(string userName, string password)
		{
			return new Core.Models.Authentication.AuthenticationModel() { Status = Core.Enumerations.Authentication.AuthenticationStatus.FailedAuthentication, Message = "External user authentication not supported in the demo environment" };
		}

		public string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName)
		{
			throw new NotImplementedException();
		}

        public void DeleteUser(string emailAddress) {
            throw new NotImplementedException();
        }
        
        public string GetNewUserName(string emailAddress)
		{
			throw new NotImplementedException();
		}

		public System.DirectoryServices.AccountManagement.UserPrincipal GetUser(string userName)
		{
			throw new NotImplementedException();
		}

		public void ExpirePassword(string emailAddress)
		{
			throw new NotImplementedException();
		}

		public bool IsPasswordExpired(string emailAddress)
		{
			throw new NotImplementedException();
		}

		public string GetUserGroup(string userName, List<string> groupNames)
		{
			throw new NotImplementedException();
		}

		public void JoinGroup(string customerName, string roleName, System.DirectoryServices.AccountManagement.UserPrincipal user)
		{
			throw new NotImplementedException();
		}

        public void UnlockAccount(string emailAddress) {
            throw new NotImplementedException();
        }

		public void UpdateUserGroups(List<string> customerNames, string roleName, string userEmail)
		{
			throw new NotImplementedException();
		}

		public bool UpdatePassword(string updatedBy, string emailAddress, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public void UpdatePassword(string emailAddress, string newPassword)
		{
			throw new NotImplementedException();
		}

		public void UpdateUserAttributes(string oldEmailAddress, string newEmailAdress, string firstName, string lastName)
		{
			throw new NotImplementedException();
		}

		public bool UsernameExists(string userName)
		{
			throw new NotImplementedException();
		}

        public bool HasAccess(string userName, string roleName) {
            throw new NotImplementedException();
        }

		public void GrantAccess(string grantedBy, string userName, string roleName)
		{
            
        }

		public void RevokeAccess(string revokedBy, string userName, string roleName)
		{
            
        }

    }
}
