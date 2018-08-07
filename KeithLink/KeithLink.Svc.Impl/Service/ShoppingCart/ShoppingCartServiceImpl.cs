﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Customers;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.PowerMenu.Order;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

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
        #endregion

        #region constructor
        public ShoppingCartServiceImpl(IShoppingCartLogic cartLogic, ICustomerRepository customerRepo, IUserProfileLogic profileLogic, IEventLogRepository log, IShipDateRepository shipDateRepo, IMinimumOrderAmountRepository minimumAmountRepo)
        {
            _shoppingCartLogic = cartLogic;
            _customerRepo = customerRepo;
            _profileLogic = profileLogic;
            _shipDateRepository = shipDateRepo;
            _log = log;
            _minimumAmountRepo = minimumAmountRepo;
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

        public ApprovedCartModel ValidateCartAmount(UserSelectedContext customer, decimal cartTotal)
        {

            ApprovedCartModel ret = new ApprovedCartModel();

            try
            {
                List<MinimumOrderAmountModel> minimumOrderAmount = _minimumAmountRepo.GetMinimumOrderAmount(customer.CustomerId, customer.BranchId);

                ret.ApprovedAmount = minimumOrderAmount[0].ApprovedAmount;

                ret.Approved = ret.ApprovedAmount < cartTotal;

                ret.Message = ret.Approved == true ?  null : "The cart total does not meet or exceed the minimum approved amount.  Please contact your DSR for more information.";
                
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
