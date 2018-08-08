using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.PowerMenu.Order;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Extensions.PowerMenu;

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
        private readonly IMinimumOrderAmountRepository _minimumAmountRepo;
        private readonly IPriceLogic _priceLogic;
        #endregion

        #region constructor
        public ShoppingCartServiceImpl(IShoppingCartLogic cartLogic, ICustomerRepository customerRepo, IUserProfileLogic profileLogic, IEventLogRepository log, IShipDateRepository shipDateRepo, IMinimumOrderAmountRepository minimumAmountRepo, IPriceLogic priceLogic)
        {
            _shoppingCartLogic = cartLogic;
            _customerRepo = customerRepo;
            _profileLogic = profileLogic;
            _shipDateRepository = shipDateRepo;
            _log = log;
            _minimumAmountRepo = minimumAmountRepo;
            _priceLogic = priceLogic;
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

        public ApprovedCartModel ValidateCartAmount(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
        {

            ApprovedCartModel ret = new ApprovedCartModel();
            Core.Models.ShoppingCart.ShoppingCart currentCart = new Core.Models.ShoppingCart.ShoppingCart();

            try
            {
                List<MinimumOrderAmountModel> minimumOrderAmount = _minimumAmountRepo.GetMinimumOrderAmount(catalogInfo.CustomerId, catalogInfo.BranchId);

                currentCart = _shoppingCartLogic.ReadCart(user, catalogInfo, cartId);

                decimal calcSubtotal = 0;

                foreach(var item in currentCart.Items)
                {
                    int qty = (int)item.Quantity;
                    int pack;
                    if (!int.TryParse(item.Pack, out pack)) { pack = 1; }
                    if (item.PackSize != null && item.PackSize.IndexOf("/") > -1)
                    { // added to aid exporting separate pack and size on cart export
                        item.Size = item.PackSize.Substring(item.PackSize.IndexOf("/") + 1);
                    }

                    calcSubtotal += (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, pack);
                }

                ret.ApprovedAmount = minimumOrderAmount[0].ApprovedAmount;

                ret.Approved = ret.ApprovedAmount <= currentCart.SubTotal;

                ret.RemainingAmount = ret.ApprovedAmount - calcSubtotal;

                if(ret.Approved == false)
                {
                    throw new Exception("The cart total does not meet or exceed the minimum approved amount.  Please contact your DSR for more information.");
                }
                
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ValidateCart", ex);
                ret.Message = ex.Message;
                ret.Approved = false;
            }

            return ret;

        }
        #endregion

        #region properties
        #endregion
    }
}
