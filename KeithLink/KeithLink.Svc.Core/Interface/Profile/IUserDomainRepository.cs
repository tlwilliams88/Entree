using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserDomainRepository
    {
        bool AuthenticateUser(string userName, string password);
        bool AuthenticateUser(string userName, string password, out string errorMessage);
        bool IsInGroup(string userName, string groupName);
    }
}
