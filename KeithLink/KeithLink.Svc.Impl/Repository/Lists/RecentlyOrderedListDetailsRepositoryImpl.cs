using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class RecentlyOrderedListDetailsRepositoryImpl : DapperDatabaseConnection, IRecentlyOrderedListDetailsRepository {
        #region constructor
        public RecentlyOrderedListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOGID = "CatalogId";

        // Change the amount of history to keep
        private const int NUMBER_TO_KEEP = 7;
        private const string PARMNAME_NUMBERTOKEEP = "NumberToKeep";
        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[ReadRecentlyOrderedDetailsbyParentId]";
        private const string SPNAME_SAVE = "[List].[SaveRecentlyOrderedDetails]";
        private const string SPNAME_DELETE = "[List].[DeleteRecentlyOrderedDetails]";
        private const string SPNAME_DELETE_OLD = "[List].[DeleteOldRecentlyOrderedDetails]";
        #endregion

        #region methods
        #endregion

        public List<RecentlyOrderedListDetail> GetRecentlyOrderedjetails(long headerId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_HEADERID, headerId);

            return Read<RecentlyOrderedListDetail>(SPNAME_GET, parms);
        }

        public long Save(RecentlyOrderedListDetail details) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, details.Id);
            parms.Add(PARMNAME_ITEMNUMBER, details.ItemNumber);
            parms.Add(PARMNAME_EACH, details.Each);
            parms.Add(PARMNAME_CATALOGID, details.CatalogId);
            parms.Add(PARMNAME_RETURNVALUE, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }

        public void DeleteRecentlyOrdered(RecentlyOrderedListDetail details) {
            
        }
    }
}