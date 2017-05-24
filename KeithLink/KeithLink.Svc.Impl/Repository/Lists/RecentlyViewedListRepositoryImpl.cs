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
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecentlyViewedListRepositoryImpl : DapperDatabaseConnection, IRecentlyViewedListRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadRecentlyViewedDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateRecentlyViewedByUserIdCustomerNumberBranch]";
        private const string COMMAND_DELETEDETAILS = "[List].[DeleteRecentlyViewedDetails]";
        #endregion
        #region constructor
        public RecentlyViewedListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ListModel> GetRecentlyViewedList(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            RecentlyViewedListHeader header = ReadOne<RecentlyViewedListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @UserId = userId, @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));

            if (header != null && headerOnly == false)
            {
                header.Items = Read<RecentlyViewedListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentRecentlyViewedHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo) };
            }
            return null;
        }

        public void AddOrUpdateRecentlyViewed(string userId,
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

        public void DeleteRecentlyViewed(string userId,
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
