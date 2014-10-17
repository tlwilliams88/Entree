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
        List<Account> GetAccountsForUser(Guid userId);
        void AddCustomerToAccount(Guid accountId, Guid customerId);
        void RemoveCustomerFromAccount(Guid accountId, Guid customerId);
        void AddUserToAccount(Guid accountId, Guid userId);
        void RemoveUserFromAccount(Guid accountId, Guid userId);
    }
}
