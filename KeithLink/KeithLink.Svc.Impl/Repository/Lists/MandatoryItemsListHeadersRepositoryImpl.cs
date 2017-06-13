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
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class MandatoryItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IMandatoryItemsListHeadersRepository
    {
        #region attributes
        private const string PARMNAME_BRANCHID = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";

        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_NAME = "Name";

        private const string SPNAME_GET = "[List].[GetMandatoryItemsHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveMandatoryItemsHeaders]";
        #endregion

        #region constructor
        public MandatoryItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public MandatoryItemsListHeader GetMandatoryItemsHeader(UserSelectedContext catalogInfo)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOne<MandatoryItemsListHeader>(SPNAME_GET, parms);
        }

        public void SaveMandatoryItemsHeader(MandatoryItemsListHeader model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, parms);
            parms.Add(PARMNAME_CUSTNUM, parms);
            parms.Add(PARMNAME_NAME, parms);
            parms.Add(PARMNAME_ID, parms);

            ExecuteCommand(SPNAME_SAVE, parms);
        }
        #endregion
    }
}
