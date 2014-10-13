using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IAccountRepository
    {
        Guid CreateAccount(string name);
        List<Account> GetAccounts();
        void AddCustomerToAccount(Guid accountId, Guid customerId);
        void AddUserToAccount(Guid accountId, Guid userId, string role);
        void RemoveUserFromAccount(Guid accountId, Guid userId);
    }
}
