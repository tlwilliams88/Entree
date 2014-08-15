using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserDomainRepository
    {
        bool AuthenticateUser(string userName, string password);
        bool IsInGroup(string userName, string groupName);
    }
}
