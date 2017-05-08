using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;

using KeithLink.Svc.Impl.Repository.DataConnection;

using Dapper;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Marketing
{
    public class CatalogCampaignItemsRepositoryImpl : DapperDatabaseConnection, ICatalogCampaignItemRepository
    {
        #region attributes
        private const string COMMAND_GET_BY_CAMPAIGN = "Marketing.GetAllCatalogCampaignItemsByHeader";

        private const string COMMAND_ADD_BY_CAMPAIGN = "Marketing.AddCatalogCampaignItemByHeader";
        private const string COMMAND_UPDATE_BY_CAMPAIGN = "Marketing.UpdateCatalogCampaignItemByHeader";
        #endregion

        #region constructor
        public CatalogCampaignItemsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        #region get
        public List<CatalogCampaignItem> GetByCampaign(Int64 campaignId)
        {
            return Read<CatalogCampaignItem>(new CommandDefinition(
                COMMAND_GET_BY_CAMPAIGN,
                new { @catalogCampaignHeaderId = campaignId },
                commandType: CommandType.StoredProcedure
            ));
        }
        #endregion

        #region save
        public void CreateOrUpdate(Int64 campaignId, CatalogCampaignItem item)
        {
            List<CatalogCampaignItem> olditems = Read<CatalogCampaignItem>(new CommandDefinition(
                                                COMMAND_GET_BY_CAMPAIGN,
                                                new { @catalogCampaignHeaderId = campaignId },
                                                commandType: CommandType.StoredProcedure
                                              ));

            var exists = olditems.Find(olditem => olditem.ItemNumber.Equals(item.ItemNumber));

            if (olditems.Find(olditem => olditem.ItemNumber.Equals(item.ItemNumber)) == null)
            {
                ExecuteCommand(new CommandDefinition(
                    COMMAND_ADD_BY_CAMPAIGN,
                    new
                    {
                        @ItemNumber = item.ItemNumber,
                        @CatalogCampaignHeaderId = campaignId
                    },
                    commandType: CommandType.StoredProcedure
                ));
            }
            else
            {
                ExecuteCommand(new CommandDefinition(
                    COMMAND_UPDATE_BY_CAMPAIGN,
                    new
                    {
                        @ItemNumber = item.ItemNumber,
                        @CatalogCampaignHeaderId = campaignId,
                        @Active = item.Active
                    },
                    commandType: CommandType.StoredProcedure
                ));
            }
        }
        #endregion
    }
}
