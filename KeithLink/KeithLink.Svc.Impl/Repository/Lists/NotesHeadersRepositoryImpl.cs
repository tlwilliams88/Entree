using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class NotesHeadersRepositoryImpl : DapperDatabaseConnection, INotesHeadersListRepository {
        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_CUSTOMERNUMBER = "CustomerNumber";
        private const string PARMNAME_BRANCHID = "BranchId";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[GetNotesHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveNotesHeader]";
        #endregion

        #region constructor
        public NotesHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public NotesListHeader GetHeadersForCustomer(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTOMERNUMBER, catalogInfo.CustomerId);

            return ReadOne<NotesListHeader>(SPNAME_GET, parms);
        }

        public long Save(NotesListHeader header) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, header.Id);
            parms.Add(PARMNAME_BRANCHID, header.BranchId);
            parms.Add(PARMNAME_CUSTOMERNUMBER, header.CustomerNumber);
            parms.Add(PARMNAME_RETURNVALUE, 0, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);

        }
        #endregion
    }
}