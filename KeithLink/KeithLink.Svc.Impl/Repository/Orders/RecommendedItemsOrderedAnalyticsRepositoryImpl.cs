using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Customers
{
    public class RecommendedItemsOrderedAnalyticsRepositoryImpl : DapperDatabaseConnection, IRecommendedItemsOrderedAnalyticsRepository
    {
        #region attributes
        #endregion

        #region constructor
        public RecommendedItemsOrderedAnalyticsRepositoryImpl() : base(Configuration.BEKDBConnectionString){ }
        #endregion

        #region methods
        public List<string> GetOrderSources()
        {
            List<string> orderSources = this.Query<string>(
                                                @"select OrderSource
                                                    from Orders.RecommendedOrderSource").ToList();

            return orderSources;
        }
        public void Add(string orderNumber, string itemNumber, char caseOrPackage, string orderSource) {
            var orderSourceId = this.Query<int>(
                                                @"select OrderSourceId
                                                    from Orders.RecommendedOrderSource
                                                    where OrderSource = @OrderSource",
                                                 new
                                                 {
                                                     OrderSource = orderSource
                                                 }).FirstOrDefault();

            this.Execute(
                         @"INSERT INTO [Orders].[RecommendedItemsOrderedAnalytics]([ControlNumber],[ItemNumber],[UnitOfMeasure],[OrderSourceId])
                               VALUES(@OrderNumber,@ItemNumber,@UnitOfMeasure,@OrderSourceiD)",
                         new
                         {
                             OrderNumber = orderNumber,
                             ItemNumber = itemNumber,
                             UnitOfMeasure = caseOrPackage,
                             OrderSourceId = orderSourceId
                         });
        }
        #endregion
    }
}
