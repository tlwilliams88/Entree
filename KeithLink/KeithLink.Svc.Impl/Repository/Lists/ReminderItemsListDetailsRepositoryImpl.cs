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
    public class ReminderItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IRemindersListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadReminderDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateReminderByCustomerNumberBranch]";
        private const string COMMAND_DELETEDETAILS = "[List].[DeleteReminderItemDetails]";
        #endregion
        #region constructor
        public ReminderItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ReminderItemsListDetail> GetRemindersDetails(long parentHeaderId)
        {
            return Read<ReminderItemsListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentRemindersHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateReminder(string customerNumber,
                                string branchId,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDDETAIL,
                new
                {
                    @CustomerNumber = customerNumber,
                    @BranchId = branchId,
                    @ItemNumber = itemNumber,
                    @Each = each,
                    @CatalogId = catalogId,
                    @Active = active
                }, commandType: CommandType.StoredProcedure));   
        }

        public void DeleteReminders(string userId,
                                string customerNumber,
                                string branchId)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_DELETEDETAILS,
                new
                {
                    @UserId = userId,
                    @CustomerNumber = customerNumber,
                    @BranchId = branchId
                }, commandType: CommandType.StoredProcedure));
        }
        #endregion
    }
}
