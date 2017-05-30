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
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class InventoryValuationListDetailsRepositoryImpl : DapperDatabaseConnection, IInventoryValuationListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadInventoryValuationListDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateInventoryValuationItemByCustomerNumberBranch]";
        #endregion
        #region constructor
        public InventoryValuationListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<InventoryValuationListDetail> GetInventoryValuationDetails(long parentHeaderId)
        {
            return Read<InventoryValuationListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentInventoryValuationListHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateRecommendedItem(string customerNumber,
                                string branchId,
                                long listId,
                                string listName,
                                string itemNumber,
                                bool each,
                                decimal quantity,
                                string catalogId,
                                bool active)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDDETAIL,
                new
                {
                    @CustomerNumber = customerNumber,
                    @BranchId = branchId,
                    @ListId = listId,
                    @ListName = listName,
                    @ItemNumber = itemNumber,
                    @Each = each,
                    @Quantity = quantity,
                    @CatalogId = catalogId,
                    @Active = active
                }, commandType: CommandType.StoredProcedure));   
        }
        #endregion
    }
}
