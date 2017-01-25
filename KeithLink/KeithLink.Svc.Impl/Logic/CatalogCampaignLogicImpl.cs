using KeithLink.Svc.Core.Interface.Marketing;

using KeithLink.Svc.Core.Models.Marketing;

using KeithLink.Svc.Core.Extensions.Marketing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic
{
    public class CatalogCampaignLogicImpl : ICatalogCampaignLogic
    {
        #region attributes
        private readonly ICatalogCampaignHeaderRepository _campaignHeaderRepo;
        private readonly ICatalogCampaignItemRepository _campaignItemRepo;
        #endregion

        #region constructor
        public CatalogCampaignLogicImpl(ICatalogCampaignHeaderRepository headerRepo, ICatalogCampaignItemRepository itemRepo)
        {
            _campaignHeaderRepo = headerRepo;
            _campaignItemRepo = itemRepo;
        }
        #endregion

        #region functions

        #region get
        public CatalogCampaignReturnModel GetCampaign(int id)
        {
            CatalogCampaignReturnModel returnValue = new CatalogCampaignReturnModel();
            returnValue = _campaignHeaderRepo.GetHeader(id).ToModel();
            returnValue.Items = _campaignItemRepo.GetByCampaign(id);

            return returnValue;
        }
        #endregion

        #endregion
    }
}
