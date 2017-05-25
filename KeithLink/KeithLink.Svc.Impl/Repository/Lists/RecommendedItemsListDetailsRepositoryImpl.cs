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
using KeithLink.Svc.Core.Models.Lists.RecommendedItem;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecommendedItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IRecommendedItemsListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadRecommendedItemDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateRecommendedItemByCustomerNumberBranch]";
        private const string COMMAND_DELETEDETAILS = "[List].[DeleteRecommendedItemsDetails]";
        #endregion
        #region constructor
        public RecommendedItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<RecommendedItemsListDetail> GetRecommendedItemsDetails(long parentHeaderId)
        {
            return Read<RecommendedItemsListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentRecommendedItemsHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateRecommendedItem(string customerNumber,
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

        public void DeleteRecommendedItems(string userId,
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
