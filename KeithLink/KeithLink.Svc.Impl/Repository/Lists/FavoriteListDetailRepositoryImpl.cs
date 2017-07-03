using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class FavoriteListDetailRepositoryImpl : DapperDatabaseConnection, IFavoriteListDetailsRepository {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_LABEL = "Label";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_LINENUMBER = "LineNumber";
        private const string PARMNAME_RETVAL = "ReturnValue";

        private const string SPNAME_DELETE = "[List].[DeleteFavoriteDetail]";
        private const string SPNAME_GETALL = "[List].[ReadFavoritesDetailsByParentId]";
        private const string SPNAME_GETONE = "[List].[GetFavoriteDetail]";
        private const string SPNAME_SAVE = "[List].[SaveFavoriteDetails]";
        #endregion

        #region ctor
        public FavoriteListDetailRepositoryImpl() : base(Configuration.BEKDBConnectionString) {}
        #endregion

        #region methods

        public void DeleteFavoriteListDetail(long id) {
            ExecuteCommand(SPNAME_DELETE, PARMNAME_ID, id);
        }

        public FavoritesListDetail GetFavoriteDetail(long id) {
            return ReadOne<FavoritesListDetail>(SPNAME_GETONE, PARMNAME_ID, id);
        }

        public List<FavoritesListDetail> GetFavoritesListDetails(long headerId) {
            return Read<FavoritesListDetail>(SPNAME_GETALL, PARMNAME_HEADERID, headerId);
        }

        public long SaveFavoriteListDetail(FavoritesListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOGID, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_ITEMNUMBER, model.ItemNumber);
            parms.Add(PARMNAME_LINENUMBER, model.LineNumber);
            parms.Add(PARMNAME_LABEL, model.Label);
            parms.Add(PARMNAME_RETVAL, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }
        #endregion
    }
}
