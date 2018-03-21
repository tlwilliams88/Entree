﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Core.Interface.Customers
{
    public interface IRecommendedItemsRepository {
        List<RecommendedItemsModel> GetRecommendedItemsForCustomer(string customernumber,
                                                                   string branch,
                                                                   List<ShoppingCartItem> cartItems,
                                                                   int numberItems = 4);
    }
}
