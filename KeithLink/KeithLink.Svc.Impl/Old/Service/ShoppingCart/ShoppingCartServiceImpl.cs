using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Interface.Cart;
using Entree.Core.Interface.Profile;
using Entree.Core.Interface.Orders;

using Entree.Core.Models.Customers;
using Entree.Core.Models.Orders;
using Entree.Core.Models.Profile;
using Entree.Core.Models.PowerMenu.Order;
using Entree.Core.Models.ShoppingCart;
using Entree.Core.Models.SiteCatalog;

using Entree.Core.Extensions.PowerMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Service.ShoppingCart
{
    public class ShoppingCartServiceImpl : IShoppingCartService
    {
        #region attributes
        private readonly ICustomerRepository _customerRepo;
        private readonly IEventLogRepository _log;
        private readonly IShoppingCartLogic _shoppingCartLogic;
        private readonly IShipDateRepository _shipDateRepository;
        private readonly IUserProfileLogic _profileLogic;
        #endregion

        #region constructor
        public ShoppingCartServiceImpl(IShoppingCartLogic cartLogic, ICustomerRepository customerRepo, IUserProfileLogic profileLogic, IEventLogRepository log, IShipDateRepository shipDateRepo)
        {
            _shoppingCartLogic = cartLogic;
            _customerRepo = customerRepo;
            _profileLogic = profileLogic;
            _shipDateRepository = shipDateRepo;
            _log = log;
        }
        #endregion

        #region functions
        public Guid ImportFromPowerMenu(VendorPurchaseOrderRequest powerMenuRequest)
        {
            UserProfile user = _profileLogic.GetUserProfile(powerMenuRequest.Login.Username).UserProfiles.FirstOrDefault();

            Core.Models.ShoppingCart.ShoppingCart newCart = new Core.Models.ShoppingCart.ShoppingCart();
            newCart.Items = new List<ShoppingCartItem>();

            // Get the customer information needed
            List<Customer> customers = _customerRepo.GetCustomersForUser(user.UserId);
            Customer customer = customers.Distinct()
                .Where(x => 
                    x.CustomerNumber.Equals(powerMenuRequest.Order.OrderHeader.CustomerNumber) &&
                    x.IsPowerMenu.Equals(true)
                )
                .FirstOrDefault();

            // Set the selected user context
            UserSelectedContext context = new UserSelectedContext();
            context.BranchId = customer.CustomerBranch;
            context.CustomerId = customer.CustomerNumber;

            // Build the generated cart
            newCart.BranchId = customer.CustomerBranch;
            newCart.PONumber = string.Format("eMenuManage Order {0} ", powerMenuRequest.Order.OrderHeader.PurchaseOrderNumber);
            newCart.Name = string.Format("eMenuManage Order - {0}", DateTime.Now.ToString());

            List<ShoppingCartItem> shoppingCartItems = powerMenuRequest.Order.OrderItem.ToShoppingCartItems(context.BranchId.ToLower());
            newCart.Items.AddRange(shoppingCartItems);

            ShipDateReturn validDates = _shipDateRepository.GetShipDates(context);

            newCart.RequestedShipDate = validDates.ShipDates.FirstOrDefault().Date;

            return _shoppingCartLogic.CreateCart(user, context, newCart);
        }
        #endregion

        #region properties
        #endregion
    }
}
