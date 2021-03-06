﻿using System.Linq;

using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Service
{
    public class CatalogCampaignServiceImpl : ICatalogCampaignService
    {
        #region attributes
        private readonly ICatalogLogic _catalogLogic;
        private readonly ICatalogCampaignLogic _campaignLogic;
        #endregion

        #region constructor
        public CatalogCampaignServiceImpl(ICatalogLogic catalogLogic, ICatalogCampaignLogic campaignLogic) {
            _catalogLogic  = catalogLogic;
            _campaignLogic = campaignLogic;
        }
        #endregion

        #region functions
        public ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel model, UserProfile profile)
        {
            CatalogCampaignReturnModel campaign = _campaignLogic.GetCampaignByUri(campaignUri);

            ProductsReturn products = null;
            var items = campaign?.Items?.Select(x => x.ItemNumber).ToList();
            if (items != null)
            {
                products = _catalogLogic.GetProductsByItemNumbers(context, items, model, profile);
            }

            return products;
        }
        #endregion

    }
}
