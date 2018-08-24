using System;
using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Extensions.Marketing;
using KeithLink.Svc.Impl.Seams;

namespace KeithLink.Svc.Impl.Logic {
    public class CatalogCampaignLogicImpl : ICatalogCampaignLogic {
        #region attributes
        private readonly ICatalogCampaignHeaderRepository _campaignHeaderRepo;
        private readonly ICatalogCampaignItemRepository _campaignItemRepo;
        private readonly ICampaignCustomerRepository _customerRepo;
        #endregion

        #region constructor
        public CatalogCampaignLogicImpl(ICatalogCampaignHeaderRepository headerRepo, ICatalogCampaignItemRepository itemRepo, 
                                        ICampaignCustomerRepository campaignCustomerRepository) {
            _customerRepo = campaignCustomerRepository;
            _campaignHeaderRepo = headerRepo;
            _campaignItemRepo = itemRepo;
        }
        #endregion

        #region functions
        public CatalogCampaignReturnModel GetCampaign(int id, bool includeItems = true)
        {
            CatalogCampaignReturnModel campaign = new CatalogCampaignReturnModel();

            var header = _campaignHeaderRepo.GetHeader(id);
            if (header != null)
            {
                campaign = header.ToModel(BEKConfiguration.CatalogCampaignImagesUrl);

                if (includeItems)
                {
                    campaign.Items = _campaignItemRepo.GetByCampaign(id);
                }
            }
            else
            {
                throw new KeyNotFoundException("A catalog campaign header was not found for id " + id);
            }

            return campaign;
        }

        public CatalogCampaignReturnModel GetCampaignByUri(string uri, bool includeItems = true)
        {
            CatalogCampaignReturnModel campaign = new CatalogCampaignReturnModel();

            var header = _campaignHeaderRepo.GetByUri(uri);
            if (header != null)
            {
                campaign = header.ToModel(BEKConfiguration.CatalogCampaignImagesUrl);

                if (includeItems)
                {
                    campaign.Items = _campaignItemRepo.GetByCampaign(campaign.Id);
                }
            }
            else
            {
                throw new KeyNotFoundException("A catalog campaign header was not found for id " + campaign.Id);
            }

            return campaign;
        }

        public CatalogCampaignsReturnModel GetAllAvailableCampaigns(UserSelectedContext context) {
            List<CatalogCampaignHeader> allCampaigns = _campaignHeaderRepo.GetAll();
            CatalogCampaignsReturnModel retVal = new CatalogCampaignsReturnModel();

            if(allCampaigns?.Count > 0) {
                IEnumerable<CatalogCampaignHeader> nonFilteredHeaders = allCampaigns.Where(c => c.HasFilter == false);
                
                if(nonFilteredHeaders?.Count() > 0) { retVal.campaigns.AddRange(nonFilteredHeaders.ToWebModel(BEKConfiguration.CatalogCampaignImagesUrl)); }

                foreach(CatalogCampaignHeader header in allCampaigns.Where(c => c.HasFilter)) {
                    List<CampaignCustomer> customers = _customerRepo.GetAllCustomersByCampaign(header.Id);

                    if(customers.Any(c => c.BranchId.Equals(context.BranchId, StringComparison.CurrentCultureIgnoreCase) &&
                                          c.CustomerNumber.Equals(context.CustomerId))) {
                        retVal.campaigns.Add(header.ToModel(BEKConfiguration.CatalogCampaignImagesUrl));
                    }
                }
            }

            return retVal;
        }
        public CatalogCampaignsReturnModel GetAllCampaigns(bool includeItems = true)
        {
            CatalogCampaignsReturnModel returnValue = new CatalogCampaignsReturnModel();
            returnValue.campaigns = _campaignHeaderRepo.GetAll()
                                                       .Select(i => i.ToModel(BEKConfiguration.CatalogCampaignImagesUrl))
                                                       .ToList();

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
