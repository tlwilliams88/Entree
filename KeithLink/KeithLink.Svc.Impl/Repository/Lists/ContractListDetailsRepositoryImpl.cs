using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Amazon.CognitoIdentity.Model;
using CommerceServer.Core.Inventory;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ContractListDetailsRepositoryImpl : DapperDatabaseConnection, IContractListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[GetContractItemsByParentContractHeaderId]";
        #endregion
        #region constructor
        public ContractListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ContractListDetail> ReadContractListDetails(long parentHeaderId)
        {
            return Read<ContractListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentContractHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }
        #endregion
    }
}
