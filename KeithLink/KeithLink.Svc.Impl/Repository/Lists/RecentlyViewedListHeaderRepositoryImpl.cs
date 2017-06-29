using System;
using System.Data;

using Dapper;

using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class RecentlyViewedListHeadersRepositoryImpl : DapperDatabaseConnection, IRecentlyViewedListHeadersRepository {
        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_BRANCHID = "BranchId";
        private const string PARMNAME_CUSTOMERNUMBER = "CustomerNumber";
        private const string PARMNAME_USERID = "UserId";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveRecentlyViewedHeaders]";
        #endregion

        #region constructor
        public RecentlyViewedListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public RecentlyViewedListHeader GetRecentlyViewedHeader(Guid userId, UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTOMERNUMBER, catalogInfo.CustomerId);
            parms.Add(PARMNAME_USERID, userId);

            return ReadOne<RecentlyViewedListHeader>(SPNAME_GET, parms);
        }

        public long Save(RecentlyViewedListHeader header, Guid userId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, header.Id);
            parms.Add(PARMNAME_USERID, userId);
            parms.Add(PARMNAME_BRANCHID, header.BranchId);
            parms.Add(PARMNAME_CUSTOMERNUMBER, header.CustomerNumber);
            parms.Add(PARMNAME_RETURNVALUE, 0, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);

        }
        #endregion
    }
}