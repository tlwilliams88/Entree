using System;
using System.Data;

using Dapper;

using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class RecentlyOrderedListHeadersRepositoryImpl : DapperDatabaseConnection, IRecentlyOrderedListHeadersRepository {
        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_BRANCHID = "BranchId";
        private const string PARMNAME_CUSTOMERNUMBER = "CustomerNumber";
        private const string PARMNAME_USERID = "UserId";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[GetRecentlyOrderedHeaderByUserIdCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveRecentlyOrderedHeader]";
        #endregion

        #region constructor
        public RecentlyOrderedListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public RecentlyOrderedListHeader GetRecentlyOrderedHeader(Guid userId, UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_USERID, userId);
            parms.Add(PARMNAME_BRANCHID, (catalogInfo.BranchId));
            parms.Add(PARMNAME_CUSTOMERNUMBER, catalogInfo.CustomerId);

            return ReadOne<RecentlyOrderedListHeader>(SPNAME_GET, parms);
        }

        public long Save(RecentlyOrderedListHeader header, Guid userId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, header.Id);
            parms.Add(PARMNAME_USERID, userId);
            parms.Add(PARMNAME_BRANCHID, header.BranchId);
            parms.Add(PARMNAME_CUSTOMERNUMBER, header.CustomerNumber);
            parms.Add(PARMNAME_RETURNVALUE, null, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);

        }
        #endregion
    }
}