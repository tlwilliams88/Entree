using KeithLink.Svc.Core.Models.Profile;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ICustomerDomainRepository {
        bool AuthenticateUser(string userName, string password);
        bool AuthenticateUser(string userName, string password, out string errorMessage);

        string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName);

        string GetNewUserName(string emailAddress);

        UserPrincipal GetUser(string userName);

        bool IsInGroup(string userName, string groupName);

        bool UpdatePassword(string emailAddress, string oldPassword, string newPassword);

        void UpdateUserAttributes(string oldEmailAddress, string newEmailAdress, string firstName, string lastName);

        bool UsernameExists(string userName);
    }
}
