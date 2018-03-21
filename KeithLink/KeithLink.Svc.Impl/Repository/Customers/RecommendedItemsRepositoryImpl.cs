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

            SortedDictionary<string,string> dict = new SortedDictionary<string, string>();
            if (cartItems != null && cartItems.Count>0) {
                foreach (var entry in cartItems)
                {
                    if (dict.ContainsKey(entry.ItemNumber) == false) {
                        dict.Add(entry.ItemNumber, entry.ItemNumber);    
                    }                    
                }
            }
            
            var parameters = new RecommendedItemsParametersModel() {
                CallSize = numberItems + 5,
                CustomerNumber = customernumber,
                BranchId = branch,
                CartItemsList = dict.Keys.ToList()
            };

            List<RecommendedItemsModel> recommended = QueryFirstRecommended(parameters);

            parameters.SkipSize = parameters.CallSize;

            list = recommended.Take(numberItems)
                              .ToList();

            while (list.Count < numberItems) {

                parameters.CallSize = 5;

                recommended = QueryNextRecommended(parameters);

                parameters.SkipSize += parameters.CallSize;

                if (recommended.Count == 0) {
                    break;
                }

                list.AddRange(recommended.Take(numberItems-list.Count));

            }

            return list;
        }

        private List<RecommendedItemsModel> QueryFirstRecommended(RecommendedItemsParametersModel parameters) {
            var recommended = this.QueryInline<RecommendedItemsModel>(
                                                                      "SELECT " +
                                                                      "TOP " + parameters.CallSize + " " +
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
        private List<RecommendedItemsModel> QueryNextRecommended(RecommendedItemsParametersModel parameters)
        {
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
                                                                      "ORDER BY ri.Confidence DESC " +
                                                                      "OFFSET " + parameters.SkipSize + " ROWS " +
                                                                      "FETCH NEXT " + parameters.CallSize + " ROWS ONLY ",
                                                                      parameters)
                                  .ToList();
            return recommended;
        }
        #endregion
    }
}
