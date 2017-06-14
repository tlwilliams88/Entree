using System.Data;

using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class MandatoryItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IMandatoryItemsListHeadersRepository {
        #region constructor
        public MandatoryItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region attributes
        private const string PARMNAME_BRANCHID = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_NAME = "Name";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[GetMandatoryItemsHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveMandatoryItemsHeaders]";
        #endregion

        #region methods
        public MandatoryItemsListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOne<MandatoryItemsListHeader>(SPNAME_GET, parms);
        }

        public long SaveMandatoryItemsHeader(MandatoryItemsListHeader model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_NAME, model.Name);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_RETURNVALUE, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }
        #endregion
    }
}