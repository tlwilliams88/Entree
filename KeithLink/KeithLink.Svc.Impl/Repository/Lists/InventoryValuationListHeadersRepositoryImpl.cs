using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class InventoryValuationListHeadersRepositoryImpl : DapperDatabaseConnection, IInventoryValuationListHeadersRepository
    {
        #region attributes
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_NAME = "Name";
        private const string PARMNAME_ACTIVE = "Active";

        private const string PARMNAME_RETVAL = "ReturnValue";

        private const string SPNAME_GETONE = "[List].[GetInventoryValuationListHeaderById]";
        private const string SPNAME_GETALL = "[List].[GetInventoryValuationListHeadersByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveInventoryValuationHeader]";
        #endregion

        #region constructor
        public InventoryValuationListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public InventoryValuationListHeader GetInventoryValuationListHeader(long id) {
            return ReadOne<InventoryValuationListHeader>(SPNAME_GETONE, PARMNAME_ID, id);
        }

        public List<InventoryValuationListHeader> GetInventoryValuationListHeaders(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return Read<InventoryValuationListHeader>(SPNAME_GETALL, parms);
        }

        public long SaveInventoryValudationListHeader(InventoryValuationListHeader model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_NAME, model.Name);
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_RETVAL, 0, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }
        #endregion
    }
}
