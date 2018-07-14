using Entree.Core.Models.Profile;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;

namespace Entree.Core.Interface.Profile
{
    public interface IUserDomainRepository
    {
        bool AuthenticateUser(string userName, string password);
        bool AuthenticateUser(string userName, string password, out string errorMessage);

        UserPrincipal GetUser(string userName);

        bool IsInGroup(string userName, string groupName);

        List<string> GetAllGroupsUserBelongsTo(UserPrincipal user, List<string> groupNames);
    }
}
