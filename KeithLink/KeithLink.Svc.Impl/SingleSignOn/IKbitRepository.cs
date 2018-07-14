using Entree.Core.Models.SiteCatalog;
using System;

namespace Entree.Core.Interface.SingleSignOn {
    public interface IKbitRepository {
        void AddCustomerToUser(string userName, UserSelectedContext customer);

        void DeleteAllCustomersForUser(string userName);

    }
}
