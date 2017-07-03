using System;
using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class RecentlyViewedListDetailsRepositoryImpl : DapperDatabaseConnection, IRecentlyViewedListDetailsRepository {
        #region constructor
        public RecentlyViewedListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_LINENUMBER = "LineNumber";

        private const string PARMNAME_NUMBERTOKEEP = "NumberToKeep";
        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[ReadRecentlyViewedDetailsbyHeaderId]";
        private const string SPNAME_SAVE = "[List].[SaveRecentlyViewedDetails]";
        private const string SPNAME_DELETE = "[List].[DeleteRecentlyViewedDetails]";
        private const string SPNAME_DELETE_OLD = "[List].[DeleteOldRecentlyViewedDetails]";
        #endregion

        #region methods
        public List<RecentlyViewedListDetail> GetRecentlyViewedDetails(long headerId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_HEADERID, headerId);

            return Read<RecentlyViewedListDetail>(SPNAME_GET, parms);
        }

        public long Save(RecentlyViewedListDetail details) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, details.Id);
            parms.Add(PARMNAME_HEADERID, details.HeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, details.ItemNumber);
            parms.Add(PARMNAME_EACH, details.Each);
            parms.Add(PARMNAME_CATALOGID, details.CatalogId);
            parms.Add(PARMNAME_LINENUMBER, details.LineNumber);

            parms.Add(PARMNAME_RETURNVALUE, 0, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }

        public void DeleteRecentlyViewed(RecentlyViewedListDetail details) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, details.Id);

            ExecuteCommand(SPNAME_DELETE, parms);
        }

        public void DeleteOldRecentlyViewed(long headerId, int numberToKeep = 7) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_HEADERID, headerId);
            parms.Add(PARMNAME_NUMBERTOKEEP, numberToKeep);

            ExecuteCommand(SPNAME_DELETE_OLD, parms);
        }
        #endregion
    }
}