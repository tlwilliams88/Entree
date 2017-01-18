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
        #endregion

        #region constructor
        public CatalogCampaignItemsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        #region get
        public List<CatalogCampaignItem> GetByCampaign(int campaignId)
        {
            return Read<CatalogCampaignItem>(new CommandDefinition(
                COMMAND_GET_BY_CAMPAIGN,
                new { @catalogCampaignHeaderId = campaignId },
                commandType: CommandType.StoredProcedure
            ));
        }
        #endregion
    }
}
