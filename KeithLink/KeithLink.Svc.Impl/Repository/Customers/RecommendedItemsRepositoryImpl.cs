using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Customers
{
    public class RecommendedItemsRepositoryImpl : DapperDatabaseConnection, IRecommendedItemsRepository
    {
        #region attributes
        #endregion

        #region constructor
        public RecommendedItemsRepositoryImpl() : base(Configuration.BEKDBConnectionString){ }
        #endregion

        #region methods
        public List<RecommendedItemsModel> GetRecommendedItemsForCustomer(string customernumber, 
                                                                          string branchId, 
                                                                          List<string> cartItemNumbers = null,
                                                                          int numberItems = 4) {

            if (customernumber == null | branchId == null) {
                return null;
            }

            List<RecommendedItemsModel> list = QueryRecommended(numberItems, customernumber, branchId, cartItemNumbers);

            return list;
        }

        private List<RecommendedItemsModel> QueryRecommended(int size, string customernumber, string branchId, List<string> cartItemNumbers ) {
            var recommended = this.Query<RecommendedItemsModel>(
                                                                @"SELECT 
                                                                      TOP (@Size) * 
                                                                      FROM Customers.RecommendedItems ri 
                                                                      INNER JOIN Customers.RecommendedItemContexts ric ON ric.ContextKey=ri.ContextDescription 
                                                                      INNER JOIN Customers.SICMap map ON map.SIC=ric.SIC 
                                                                      WHERE map.CustomerNumber=@CustomerNumber 
                                                                      AND map.BranchId=@BranchId 
                                                                      AND ri.ItemNumber NOT IN (@CartItemsList) 
                                                                      AND ri.RecommendedItem NOT IN (@CartItemsList) 
                                                                      ORDER BY ri.Confidence DESC ",
                                                                  new {
                                                                      Size = size,
                                                                      CustomerNumber = customernumber,
                                                                      BranchId = branchId,
                                                                      CartItemsList = (cartItemNumbers!=null) ? cartItemNumbers : new List<string>()
                                                                  })
                                  .ToList();
            return recommended;
        }
        #endregion
    }
}
