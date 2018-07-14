using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Models.Customers;
using Entree.Core.Models.Orders;
using Entree.Core.Models.ShoppingCart;

namespace Entree.Core.Interface.Orders
{
    public interface IRecommendedItemsOrderedAnalyticsRepository {
        void Add(string itemNumber, char caseOrPackage, string orderSource, string cartId, string productGroupingInsightKey);
        void DeleteByCartIdAndItemNumber(string cartId, string itemNumber);
        List<string> GetOrderSources();
        void UpdateAnalyticsForCardIdWithControlNumber(string cartId, string controlNumber);
    }
}
