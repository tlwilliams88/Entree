using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IUserActiveCartLogic {
        UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId);

        void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId);

    }
}
