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
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "ParentFavoritesHeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_LABEL = "Label";
        private const string PARMNAME_ITEMNUM = "ItemNumber";

        private const string SPNAME_DELETE = "[List].[DeleteFavoriteDetail]";
        private const string SPNAME_GET = "[List].[ReadFavoritesDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[AddOrUpdateFavoriteByUserIdCustomerNumberBranch]";
        #endregion

        #region ctor
        public FavoriteListDetailRepositoryImpl() : base(Configuration.BEKDBConnectionString) {}
        #endregion

        #region methods

        public void DeleteFavoriteListDetail(long id) {
            ExecuteCommand(SPNAME_DELETE, PARMNAME_ID, id);
        }

        public List<FavoritesListDetail> GetFavoritesListDetails(long headerId) {
            return Read<FavoritesListDetail>(SPNAME_GET, PARMNAME_HEADERID, headerId);
        }

        public void SaveFavoriteListDetail(FavoritesListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOGID, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_ITEMNUM, model.ItemNumber);
            parms.Add(PARMNAME_LABEL, model.Label);

            ExecuteCommand(SPNAME_SAVE, parms);
        }

        #endregion
    }
}
