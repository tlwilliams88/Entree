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
    public class MandatoryItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IMandatoryItemsListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadMandatoryItemDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateMandatoryItemByCustomerNumberBranch]";
        private const string COMMAND_DELETEDETAILS = "[List].[DeleteMandatoryItemDetails]";
        #endregion
        #region constructor
        public MandatoryItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<MandatoryItemsListDetail> GetMandatoryItemsDetails(long parentHeaderId)
        {
            return Read<MandatoryItemsListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentMandatoryItemsHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateMandatoryItem(string customerNumber,
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

        public void DeleteMandatoryItems(string userId,
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
