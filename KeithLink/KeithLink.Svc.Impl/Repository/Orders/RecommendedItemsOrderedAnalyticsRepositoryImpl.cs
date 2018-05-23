using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using KeithLink.Svc.Core.Interface.DataConnection;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Customers
{
    public class RecommendedItemsOrderedAnalyticsRepositoryImpl : IRecommendedItemsOrderedAnalyticsRepository
    {
        #region attributes
        private IDapperDatabaseConnection connection;
        #endregion

        #region constructor
        public RecommendedItemsOrderedAnalyticsRepositoryImpl(IDapperDatabaseConnection dbConnection) {
            connection = dbConnection;
        }
        #endregion

        #region methods

        public void Add(string itemNumber, char caseOrPackage, string orderSource, string cartId, string productGroupingInsightKey)
        {
            var orderSourceId = connection.Query<int>(
                                                @"select OrderSourceId
                                                    from Orders.RecommendedOrderSource
                                                    where OrderSource = @OrderSource",
                                                 new
                                                 {
                                                     OrderSource = orderSource
                                                 }).FirstOrDefault();

            connection.Execute(
                         @"INSERT INTO [Orders].[RecommendedItemsOrderedAnalytics]
                            ([CartId], [ControlNumber],[ItemNumber],[UnitOfMeasure],[OrderSourceId])
                          VALUES
                            (@CartId, @ControlNumber,@ItemNumber,@UnitOfMeasure,@OrderSourceiD)",
                         new
                         {
                             CartId = cartId,
                             ItemNumber = itemNumber,
                             ProductGroupingInsightKey = productGroupingInsightKey,
                             UnitOfMeasure = caseOrPackage,
                             OrderSourceId = orderSourceId
                         });
        }

        public List<string> GetOrderSources()
        {
            List<string> orderSources = connection.Query<string>(
                                                @"select OrderSource
                                                    from Orders.RecommendedOrderSource").ToList();

            return orderSources;
        }

        public void UpdateAnalyticsForCardIdWithControlNumber(string cartId, string controlNumber) {
            connection.Execute(@"UPDATE Orders.REcommendedItemsOrderedAnalytics
                                SET 
                                    ControlNumber = @ControlNumber
                                WHERE
                                    CartId = @CartId", new { CartId = cartId, ControlNumber = controlNumber});
        }
        #endregion
    }
}
