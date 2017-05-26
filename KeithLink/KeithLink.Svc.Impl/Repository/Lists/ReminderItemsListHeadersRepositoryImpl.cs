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
using KeithLink.Svc.Core.Models.Lists.ReminderItem;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ReminderItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IRemindersListHeadersRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetRemindersHeaderByCustomerNumberBranch]";
        #endregion
        #region constructor
        public ReminderItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public ReminderItemsListHeader GetReminderItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return ReadOne<ReminderItemsListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));
        }
        #endregion
    }
}
