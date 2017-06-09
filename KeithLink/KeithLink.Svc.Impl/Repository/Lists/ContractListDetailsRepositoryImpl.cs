using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ContractListDetailsRepositoryImpl : DapperDatabaseConnection, IContractListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_HEADERID = "ParentContractHeaderId";

        private const string SPNAME_GET = "[List].[GetContractItemsByParentContractHeaderId]";
        #endregion

        #region constructor
        public ContractListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString){ }
        #endregion

        #region methods
        public List<ContractListDetail> GetContractListDetails(long parentHeaderId) {
            return Read<ContractListDetail>(SPNAME_GET, PARMNAME_HEADERID, parentHeaderId);
        }
        #endregion

    }
}
