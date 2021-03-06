﻿using KeithLink.Common.Core.Interfaces.Logging;
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

using KeithLink.Svc.Impl.Logic.Orders;

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
        private readonly IOrderLogic _orderLogic;
        #endregion

        #region constructor
        public ShoppingCartServiceImpl(IShoppingCartLogic cartLogic, ICustomerRepository customerRepo, IUserProfileLogic profileLogic, IEventLogRepository log, IShipDateRepository shipDateRepo, IMinimumOrderAmountRepository minimumAmountRepo, IPriceLogic priceLogic, IOrderLogic orderLogic)
        {
            _shoppingCartLogic = cartLogic;
            _customerRepo = customerRepo;
            _profileLogic = profileLogic;
            _shipDateRepository = shipDateRepo;
            _log = log;
            _minimumAmountRepo = minimumAmountRepo;
            _priceLogic = priceLogic;
            _orderLogic = orderLogic;
        }

        public Core.Models.Orders.Order existingOrder { get; private set; }
        public Core.Models.ShoppingCart.ShoppingCart currentCart { get; private set; }
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

        public ApprovedCartModel ValidateCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, string orderNumber)
        {

            ApprovedCartModel ret = new ApprovedCartModel();

            bool isCart = cartId != null && cartId != Guid.Empty ? true : false;
            if(isCart == true)
            {
                Core.Models.ShoppingCart.ShoppingCart currentCart = new Core.Models.ShoppingCart.ShoppingCart();
            }
            else
            {
                Core.Models.Orders.Order existingOrder = new Core.Models.Orders.Order();
            }

            try
            {
                MinimumOrderAmountModel minimumOrderAmount = _minimumAmountRepo.GetMinimumOrderAmount(catalogInfo.CustomerId, catalogInfo.BranchId);

                decimal subtotal = 0;

                ret.ApprovedAmount = minimumOrderAmount.ApprovedAmount;

                if (isCart == true)
                {
                    currentCart = _shoppingCartLogic.ReadCart(user, catalogInfo, cartId);

                    subtotal = (decimal)PricingHelper.CalculateCartSubtotal(currentCart.Items);

                }
                else
                {
                    existingOrder = _orderLogic.ReadOrder(user, catalogInfo, orderNumber, false);

                    subtotal = (decimal)existingOrder.OrderTotal;
                }

                ret.Approved = ret.ApprovedAmount <= subtotal;

                ret.RemainingAmount = ret.ApprovedAmount - subtotal > 0 ? ret.ApprovedAmount - subtotal : 0;

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
