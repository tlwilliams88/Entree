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
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecentlyOrderedListDetailsRepositoryImpl : DapperDatabaseConnection, IRecentlyOrderedListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadRecentlyOrderedDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateRecentlyOrderedByUserIdCustomerNumberBranch]";
        private const string COMMAND_DELETEDETAILS = "[List].[DeleteRecentlyOrderedDetails]";
        #endregion
        #region constructor
        public RecentlyOrderedListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<RecentlyOrderedListDetail> GetRecentlyOrderedDetails(long parentHeaderId)
        {
            return Read<RecentlyOrderedListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @HeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateRecentlyOrdered(string userId,
                                string customerNumber,
                                string branchId,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDDETAIL,
                new
                {
                    @UserId = userId,
                    @CustomerNumber = customerNumber,
                    @BranchId = branchId,
                    @ItemNumber = itemNumber,
                    @Each = each,
                    @CatalogId = catalogId,
                    @Active = active,
                    @NumberToKeep = Configuration.RecentItemsToKeep
                }, commandType: CommandType.StoredProcedure));   
        }

        public void DeleteRecentlyOrdered(string userId,
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
