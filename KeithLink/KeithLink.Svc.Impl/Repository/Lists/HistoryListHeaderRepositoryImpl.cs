using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class HistoryListHeaderRepositoryImpl : DapperDatabaseConnection, IHistoryListHeaderRepository {
        #region attributes
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";

        private const string SPNAME_GETONE = "";
        #endregion

        #region ctor
        public HistoryListHeaderRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public HistoryListHeader GetHistoryListHeader(UserSelectedContext customerInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, customerInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, customerInfo.CustomerId);

            return ReadOne<HistoryListHeader>(SPNAME_GETONE, parms);
        }
        #endregion
    }
}
