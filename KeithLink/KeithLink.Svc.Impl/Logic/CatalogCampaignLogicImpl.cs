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
        public CatalogCampaignReturnModel GetCampaign(int id, bool includeItems = true)
        {
            CatalogCampaignReturnModel returnValue = new CatalogCampaignReturnModel();
            returnValue = _campaignHeaderRepo.GetHeader(id).ToModel();

            if (includeItems)
            {
                returnValue.Items = _campaignItemRepo.GetByCampaign(id);
            }

            return returnValue;
        }

        public CatalogCampaignReturnModel GetCampaignByUri(string uri, bool includeItems = true)
        {
            CatalogCampaignReturnModel returnValue = new CatalogCampaignReturnModel();
            returnValue = _campaignHeaderRepo.GetByUri(uri).ToModel();

            if (includeItems)
            {
                returnValue.Items = _campaignItemRepo.GetByCampaign(returnValue.Id);
            }

            return returnValue;
        }

        public CatalogCampaignsReturnModel GetAllCampaigns(bool includeItems = true)
        {
            CatalogCampaignsReturnModel returnValue = new CatalogCampaignsReturnModel();
            returnValue.campaigns = _campaignHeaderRepo.GetAll();

            if (includeItems)
            {
                foreach (var header in returnValue.campaigns)
                {
                    header.Items = _campaignItemRepo.GetByCampaign(header.Id);
                }
            }

            return returnValue;
        }

        public bool AddOrUpdateCampaign(CatalogCampaignAddOrUpdateRequestModel campaign)
        {
            long Id = _campaignHeaderRepo.CreateOrUpdate(campaign);

            foreach (CatalogCampaignItem item in campaign.Items)
            {
                _campaignItemRepo.CreateOrUpdate(Id, item);
            }

            return true;
        }

        #endregion
    }
}
