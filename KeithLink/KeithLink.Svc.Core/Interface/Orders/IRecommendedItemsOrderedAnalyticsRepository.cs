using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IRecommendedItemsOrderedAnalyticsRepository {
        void Add(string itemNumber, char caseOrPackage, string orderSource, string cartId, string productGroupingInsightKey);
        void DeleteByCartIdAndItemNumber(string cartId, string itemNumber);
        List<string> GetOrderSources();
        void UpdateAnalyticsForCardIdWithControlNumber(string cartId, string controlNumber);
    }
}
