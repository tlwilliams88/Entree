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
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListDetailsRepositoryImpl : DapperDatabaseConnection, ICustomListDetailsRepository
    {
        #region attributes
        private const string COMMAND_GETDETAILS = "[List].[ReadCustomListDetailsByParentId]";
        private const string COMMAND_ADDDETAIL = "[List].[AddOrUpdateCustomListItemById]";
        #endregion
        #region constructor
        public CustomListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<CustomListDetail> GetCustomListDetails(long parentHeaderId)
        {
            return Read<CustomListDetail>(new CommandDefinition(
                COMMAND_GETDETAILS,
                new { @ParentCustomListHeaderId = parentHeaderId },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void AddOrUpdateCustomListItem(
            long parentCustomListHeaderId,
            string itemNumber,
	        bool each,
            decimal par,
	        string catalogId,
            bool active)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDDETAIL,
                new
                {
                    @ParentCustomListHeaderId = parentCustomListHeaderId,
                    @ItemNumber = itemNumber,
                    @Each = each,
                    @Par = par,
                    @CatalogId = catalogId,
                    @Active = active
                }, commandType: CommandType.StoredProcedure));   
        }
        #endregion
    }
}
