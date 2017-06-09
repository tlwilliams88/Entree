using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class FavoriteListDetailRepositoryImpl : DapperDatabaseConnection, IFavoriteListDetailRepository {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "ParentFavoritesHeaderId";
        private const string PARMNAME_ITEMNUM = "ItemNumber";
        private const string PARMNAME_USERID = "UserId";

        private const string SPNAME_GET = "[List].[ReadFavoritesDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[AddOrUpdateFavoriteByUserIdCustomerNumberBranch]";
        #endregion

        #region ctor
        public FavoriteListDetailRepositoryImpl(string connectionString) : base(connectionString) {}
        #endregion

        #region methods

        public void AddOrUpdateFavorite(string userId, string customerNumber, string branchId, 
                                        string itemNumber, bool each, string catalogId, 
                                        bool active) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, active);
            parms.Add(PARMNAME_BRANCH, branchId);
            parms.Add(PARMNAME_CATALOGID, catalogId);
            parms.Add(PARMNAME_CUSTNUM, customerNumber);
            parms.Add(PARMNAME_EACH, each);
            parms.Add(PARMNAME_ITEMNUM, itemNumber);
            parms.Add(PARMNAME_USERID, userId);

            ExecuteCommand(SPNAME_SAVE, parms);
        }

        public List<FavoritesListDetail> GetFavoritesListDetails(long headerId) {
            return Read<FavoritesListDetail>(SPNAME_GET, PARMNAME_HEADERID, headerId);
        }
        #endregion
    }
}
