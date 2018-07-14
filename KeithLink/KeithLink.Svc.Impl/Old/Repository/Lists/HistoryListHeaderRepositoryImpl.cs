using Dapper;

using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists.History;
using Entree.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class HistoryListHeaderRepositoryImpl : DapperDatabaseConnection, IHistoryListHeaderRepository {
        #region attributes
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";

        private const string SPNAME_GETONE = "[List].[GetHistoryHeaderByCustomerNumberAndBranch]";
        #endregion

        #region ctor
        public HistoryListHeaderRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public HistoryListHeader GetHistoryListHeader(UserSelectedContext customerInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, customerInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, customerInfo.CustomerId);

            return ReadOneSP<HistoryListHeader>(SPNAME_GETONE, parms);
        }
        #endregion
    }
}
