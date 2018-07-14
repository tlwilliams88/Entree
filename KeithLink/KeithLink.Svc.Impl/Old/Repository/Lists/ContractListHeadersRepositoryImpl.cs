using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using Entree.Core.Extensions;
using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.Contract;

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

            return ReadOneSP<ContractListHeader>(SPNAME_GET, parms);
        }
        #endregion
    }
}
