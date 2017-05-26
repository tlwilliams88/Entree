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
        private const string COMMAND_GETHEADER = "[List].[GetMandatoryItemsHeaderByCustomerNumberBranch]";
        #endregion
        #region constructor
        public MandatoryItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public MandatoryItemsListHeader GetMandatoryItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return ReadOne<MandatoryItemsListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));
        }
        #endregion
    }
}
