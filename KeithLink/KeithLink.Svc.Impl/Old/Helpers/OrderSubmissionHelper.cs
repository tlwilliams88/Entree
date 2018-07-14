﻿using Entree.Core.Interface.Cache;
using Entree.Core.Interface.Orders;
using Entree.Core.Interface.Orders.History;
using Entree.Core.Models.SiteCatalog;
using Entree.Core.Extensions.Orders.History;
using EF = Entree.Core.Models.Orders.History.EF;
using CS = Entree.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Orders;
using Entree.Core.Extensions.Orders;
using Entree.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Helpers
{
    public class OrderSubmissionHelper
    {
        private const string CACHE_GROUPNAME = "Orders";
        private const string CACHE_NAME = "Orders";
        private const string CACHE_PREFIX = "Default";

        /// <summary>
        /// Method to set up the cross-references needed to be able to check for a cart/order being submitted
        /// </summary>
        public static void StartOrderBlock(Guid cartId, string orderNumber, ICacheRepository cache)
        {
            if (cartId != null && orderNumber != null)
            {
                string cachekey = string.Format("Cart2OrderId_{0}", cartId);
                cache.AddItem<string>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cachekey, TimeSpan.FromMinutes(5), orderNumber); // create the cart2order cross-reference
            }

        }

        /// <summary>
        /// Method to set up the cross-references needed to be able to check for a cart/order being submitted
        /// </summary>
        public static void StartChangeOrderBlock(string orderNumber, string newOrderNumber, ICacheRepository cache)
        {
            if (orderNumber != null && newOrderNumber != null)
            {
                string cachekey = string.Format("ChangeOrder_Order2NewOrderId_{0}", orderNumber);
                cache.AddItem<string>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cachekey, TimeSpan.FromMinutes(5), newOrderNumber); // create the cart2order cross-reference
            }

        }

        /// <summary>
        /// Method to check for a cart/changeorder being submitted
        /// </summary>
        public static bool CheckOrderBlock(UserProfile user, UserSelectedContext catalogInfo, Guid? cartId, string orderNumber,
            IPurchaseOrderRepository pORepository, IOrderHistoryHeaderRepsitory hHRepository, ICacheRepository cache)
        {
            if (cartId != null && orderNumber == null) // if we are given cartId but not orderNumber, check for cross-reference
            {
                string cachekey = string.Format("Cart2OrderId_{0}", cartId);
                orderNumber = cache.GetItem<string>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cachekey);
            }

            if (orderNumber != null && cartId == null) // if we are given an orderNumber but not cartId, we could be in changeorder, check for cross-reference
            {
                string cachekey = string.Format("ChangeOrder_Order2NewOrderId_{0}", orderNumber);
                string newOrderNumber = cache.GetItem<string>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cachekey);
                if(newOrderNumber != null)
                {
                    orderNumber = newOrderNumber;
                }
            }

            EF.OrderHistoryHeader theEFOrder = null;

            Order theOrder = null;

            // to check, we need to build the order and look at its status.
            if (catalogInfo != null && orderNumber != null && hHRepository != null)
            {
                // We just get the header not the details for performance
                theEFOrder = hHRepository.Read(h => h.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                            (h.ControlNumber.Equals(orderNumber))).FirstOrDefault();
            }

            if (theEFOrder == null && pORepository != null && orderNumber != null) // if we don't find a header, try and construct the order from the purchaseOrder
            {
                CS.PurchaseOrder po = pORepository.ReadPurchaseOrderByTrackingNumber(orderNumber);

                theOrder = po.ToOrder();
            }
            else
            {
                if (theEFOrder != null)
                {
                    theOrder = theEFOrder.ToOrder();
                }
            }

            // we provide for no block for sysadmin
            // block a pending or submitted order otherwise
            //if (user.RoleName.Equals("beksysadmin", StringComparison.CurrentCultureIgnoreCase) == false && 
            //    theOrder != null && ((theOrder.Status.Equals("Pending")) | (theOrder.Status.Equals("Submitted"))))
            if (theOrder != null)
            {
                if ((theOrder.Status.Equals("Pending")) | (theOrder.Status.Equals("Submitted")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
