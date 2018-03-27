using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

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
                                                                @"SELECT TOP (@Size)
                                                                      ri.ItemNumber,
                                                                      ri.RecommendedItem,
                                                                      ri.Confidence,
                                                                      ri.ContextKey,
                                                                      ri.PrimaryPriceListCode,
                                                                      ri.SecondaryPriceListCode
                                                                  FROM Customers.RecommendedItems ri 
                                                                      INNER JOIN Customers.RecommendedItemContexts ric ON ric.ContextKey=ri.ContextKey 
                                                                      INNER JOIN Customers.SICMap map ON map.SIC=ric.SIC 
                                                                  WHERE map.CustomerNumber=@CustomerNumber 
                                                                      AND map.BranchId=@BranchId 
                                                                      AND ri.ItemNumber IN @CartItemsList
                                                                      AND ri.RecommendedItem NOT IN @CartItemsList
                                                                      AND ri.PrimaryPriceListCode != ri.SecondaryPriceListCode
                                                                  ORDER BY ri.Confidence DESC ",
                                                                  new {
                                                                      Size = size,
                                                                      CustomerNumber = customernumber,
                                                                      BranchId = branchId,
                                                                      CartItemsList = GetItemNumbersAsAnsiStrings(cartItemNumbers.ToArray())
                                                                  }).ToList();

            return recommended;
        }

        private IEnumerable<DbString> GetItemNumbersAsAnsiStrings(string[] strings) {
            foreach (var ansiString in strings) {
                yield return new DbString() {IsAnsi = true, Value = ansiString, Length = 6};
            }
        }
        #endregion
    }
}
