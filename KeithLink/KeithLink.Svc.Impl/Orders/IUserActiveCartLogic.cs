using Entree.Core.Models.Orders;
using Entree.Core.Models.SiteCatalog;

using System;

namespace Entree.Core.Interface.Orders {
    public interface IUserActiveCartLogic {
        UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId);

        void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId);

    }
}
