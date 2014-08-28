using KeithLink.Svc.Core.Models.Profile;
using System;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserDomainRepository
    {
        bool AuthenticateUser(string userName, string password);
        bool AuthenticateUser(string userName, string password, out string errorMessage);
        bool IsInGroup(string userName, string groupName);
    }
}
