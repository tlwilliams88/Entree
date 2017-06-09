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
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ContractListHeadersRepositoryImpl : DapperDatabaseConnection, IContractListHeadersRepository
    {
        #region attributes

        private const string PARMNAME_BRANCHID = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";

        private const string SPNAME_GET = "[List].[GetContractHeaderByCustomerAndBranch]";
        #endregion

        #region constructor
        public ContractListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public ContractListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCHID, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOne<ContractListHeader>(SPNAME_GET, parms);
        }
        #endregion
    }
}
