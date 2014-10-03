using KeithLink.Svc.Core.Models.Profile;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserDomainRepository
    {
        bool AuthenticateUser(string userName, string password);
        bool AuthenticateUser(string userName, string password, out string errorMessage);

        UserPrincipal GetUser(string userName);

        bool IsInGroup(string userName, string groupName);
    }
}
