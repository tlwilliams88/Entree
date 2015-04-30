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
        Guid CreateAccount(string createdBy, string name);
        List<Account> GetAccounts();
        List<Account> GetAccountsForUser(Guid userId);
        void AddCustomerToAccount(string addedBy, Guid accountId, Guid customerId);
        void RemoveCustomerFromAccount(string removedBy, Guid accountId, Guid customerId);
		void AddUserToAccount(string addedBy, Guid accountId, Guid userId);
		void RemoveUserFromAccount(string removedBy, Guid accountId, Guid userId);
		Guid UpdateAccount(string name, Guid accountId);
    }
}
