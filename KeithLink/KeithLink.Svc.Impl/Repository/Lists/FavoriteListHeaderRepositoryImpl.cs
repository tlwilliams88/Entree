using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class FavoriteListHeaderRepositoryImpl : DapperDatabaseConnection, IFavoriteListHeaderRepository {
        #region attributes
        private const string SPNAME_GETHEADER = "[List].[GetFavoritesHeaderByUserIdCustomerNumberBranch]";

        private const string PARMNAME_BRANCH = "Branch";
        private const string PARMNAME_CUSTID = "CustomerId";
        private const string PARMNAME_USERID = "UserId";
        #endregion

        #region ctor
        public FavoriteListHeaderRepositoryImpl(string connectionString) : base(connectionString) {}
        #endregion

        #region method
        public FavoritesListHeader GetFavoritesList(string userId, UserSelectedContext catalogInfo)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTID, catalogInfo.CustomerId);
            parms.Add(PARMNAME_USERID, userId);

            return ReadOne<FavoritesListHeader>(SPNAME_GETHEADER, parms);
        }
        #endregion
    }
}
