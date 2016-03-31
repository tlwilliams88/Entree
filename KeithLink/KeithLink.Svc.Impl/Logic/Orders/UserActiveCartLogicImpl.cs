using KeithLink.Svc.Core.Interface.Orders;

using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class UserActiveCartLogicImpl : IUserActiveCartLogic {
        #region attributes
        private readonly IUserActiveCartRepository _repo;
        private readonly IUnitOfWork _uow;
        #endregion

        #region ctor
        public UserActiveCartLogicImpl(IUserActiveCartRepository userActiveCartRepository, IUnitOfWork unitOfWork) {
            _repo = userActiveCartRepository;
            _uow = unitOfWork;
        }
        #endregion

        #region methods
        public UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId) {
            var activeCart = _repo.GetCurrentUserCart(userId, catalogInfo.BranchId, catalogInfo.CustomerId);

            if(activeCart == null) {
                return null;
            } else {
                return new UserActiveCartModel() { UserId = activeCart.UserId, CartId = activeCart.CartId };
            }
        }

        public void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId) {
            var activeCart = _repo.GetCurrentUserCart(userId, catalogInfo.BranchId, catalogInfo.CustomerId);

            if(activeCart == null)
                _repo.Create(new UserActiveCart() { CartId = cartId, UserId = userId, CustomerId = catalogInfo.CustomerId, BranchId = catalogInfo.BranchId });
            else {
                activeCart.CartId = cartId;
                _repo.Update(activeCart);
            }

            _uow.SaveChanges();
        }
        #endregion
    }
}
