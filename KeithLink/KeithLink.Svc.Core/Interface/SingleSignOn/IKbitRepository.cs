using KeithLink.Svc.Core.Models.SiteCatalog;
using System;

namespace KeithLink.Svc.Core.Interface.SingleSignOn {
    public interface IKbitRepository {
        void AddCustomerToUser(string userName, UserSelectedContext customer);

        void DeleteAllCustomersForUser(string userName);

    }
}
