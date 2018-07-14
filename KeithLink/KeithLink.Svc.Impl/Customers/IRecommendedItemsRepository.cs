using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Models.Customers;
using Entree.Core.Models.ShoppingCart;

namespace Entree.Core.Interface.Customers
{
    public interface IRecommendedItemsRepository {
        List<RecommendedItemsModel> GetRecommendedItemsForCustomer(string customernumber,
                                                                          string branchId,
                                                                          List<string> cartItemNumbers = null,
                                                                          int numberItems = 4);
    }
}
