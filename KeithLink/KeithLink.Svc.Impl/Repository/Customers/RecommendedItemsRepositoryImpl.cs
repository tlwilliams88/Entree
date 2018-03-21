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
                                                                          string branch, 
                                                                          List<ShoppingCartItem> cartItems,
                                                                          int numberItems = 4) {

            List<RecommendedItemsModel> list = new List<RecommendedItemsModel>();

            if (customernumber == null | branch == null) {
                return null;
            }

            var parameters = new RecommendedItemsParametersModel() {
                CustomerNumber = customernumber,
                BranchId = branch,
                CartItemsList = (cartItems != null) ? cartItems.Select(c => c.ItemNumber).ToList() : new List<string>()
            };

            List<RecommendedItemsModel> recommended = QueryRecommended(parameters);

            list = recommended.Take(numberItems)
                              .ToList();

            return list;
        }

        private List<RecommendedItemsModel> QueryRecommended(RecommendedItemsParametersModel parameters) {
            var recommended = this.QueryInline<RecommendedItemsModel>(
                                                                      "SELECT " +
                                                                      "* " +
                                                                      "FROM Customers.RecommendedItems ri " +
                                                                      "INNER JOIN Customers.RecommendedItemContexts ric ON ric.ContextKey=ri.ContextDescription " +
                                                                      "INNER JOIN Customers.SICMap map ON map.SIC=ric.SIC " +
                                                                      "WHERE map.CustomerNumber=@CustomerNumber " +
                                                                      "AND map.BranchId=@BranchId " +
                                                                      "AND ri.ItemNumber NOT IN (@CartItemsList) " +
                                                                      "AND ri.RecommendedItem NOT IN (@CartItemsList) " +
                                                                      "ORDER BY ri.Confidence DESC ",
                                                                      parameters)
                                  .ToList();
            return recommended;
        }
        #endregion
    }
}
