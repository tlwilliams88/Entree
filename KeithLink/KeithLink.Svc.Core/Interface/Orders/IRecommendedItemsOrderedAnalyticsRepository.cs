using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IRecommendedItemsOrderedAnalyticsRepository {
        List<string> GetOrderSources();
        void Add(string orderNumber, string itemNumber, char caseOrPackage, string orderSource);
    }
}
