using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ContractListDetailsRepositoryImpl : DapperDatabaseConnection, IContractListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_HEADERID = "HeaderId";

        private const string SPNAME_GET = "[List].[GetContractItemsByHeaderId]";
        #endregion

        #region constructor
        public ContractListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString){ }
        #endregion

        #region methods
        public List<ContractListDetail> GetContractListDetails(long parentHeaderId) {
            return ReadSP<ContractListDetail>(SPNAME_GET, PARMNAME_HEADERID, parentHeaderId);
        }
        #endregion

    }
}
