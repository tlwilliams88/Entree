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
        private const string COMMAND_GETHEADER = "[List].[GetInventoryValuationListHeaderById]";
        private const string COMMAND_GETHEADERS = "[List].[GetInventoryValuationListHeadersByCustomerNumberBranch]";
        #endregion
        #region constructor
        public InventoryValuationListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public InventoryValuationListHeader GetInventoryValuationListHeader(long reportId)
        {
            return ReadOne<InventoryValuationListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @ListId = reportId },
                                commandType: CommandType.StoredProcedure
                            ));
        }

        public List<InventoryValuationListHeader> GetInventoryValuationListHeaders(UserSelectedContext catalogInfo)
        {
            return Read<InventoryValuationListHeader>(new CommandDefinition(
                       COMMAND_GETHEADERS,
                       new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                       commandType: CommandType.StoredProcedure
                   ));
        }
        #endregion
    }
}
