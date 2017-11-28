using System;
using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Extensions.Marketing;

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
            CatalogCampaignReturnModel returnValue = new CatalogCampaignReturnModel();
            returnValue = _campaignHeaderRepo.GetHeader(id).ToModel( Configuration.CatalogCampaignImagesUrl);

            if (includeItems)
            {
                returnValue.Items = _campaignItemRepo.GetByCampaign(id);
            }

            return returnValue;
        }

        public CatalogCampaignReturnModel GetCampaignByUri(string uri, bool includeItems = true)
        {
            CatalogCampaignReturnModel returnValue = new CatalogCampaignReturnModel();
            returnValue = _campaignHeaderRepo.GetByUri(uri).ToModel( Configuration.CatalogCampaignImagesUrl);

            if (includeItems)
            {
                returnValue.Items = _campaignItemRepo.GetByCampaign(returnValue.Id);
            }

            return returnValue;
        }

        public CatalogCampaignsReturnModel GetAllAvailableCampaigns(UserSelectedContext context) {
            List<CatalogCampaignHeader> allCampaigns = _campaignHeaderRepo.GetAll();
            CatalogCampaignsReturnModel retVal = new CatalogCampaignsReturnModel();

            if(allCampaigns?.Count > 0) {
                IEnumerable<CatalogCampaignHeader> nonFilteredHeaders = allCampaigns.Where(c => c.HasFilter == false);
                
                if(nonFilteredHeaders?.Count() > 0) { retVal.campaigns.AddRange(nonFilteredHeaders.ToWebModel(Configuration.CatalogCampaignImagesUrl)); }

                foreach(CatalogCampaignHeader header in allCampaigns.Where(c => c.HasFilter)) {
                    List<CampaignCustomer> customers = _customerRepo.GetAllCustomersByCampaign(header.Id);

                    if(customers.Any(c => c.BranchId.Equals(context.BranchId, StringComparison.CurrentCultureIgnoreCase) &&
                                          c.CustomerNumber.Equals(context.CustomerId))) {
                        retVal.campaigns.Add(header.ToModel(Configuration.CatalogCampaignImagesUrl));
                    }
                }
            }

            return retVal;
        }
        public CatalogCampaignsReturnModel GetAllCampaigns(bool includeItems = true)
        {
            CatalogCampaignsReturnModel returnValue = new CatalogCampaignsReturnModel();
            returnValue.campaigns = _campaignHeaderRepo.GetAll()
                                                       .Select(i => i.ToModel(Configuration.CatalogCampaignImagesUrl))
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
