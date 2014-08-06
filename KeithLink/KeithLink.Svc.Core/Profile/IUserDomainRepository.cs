using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    public interface IUserDomainRepository
    {
        public bool AuthenticateUser(string userName, string password);
        public bool IsInGroup(string userName, string groupName);
    }
}
