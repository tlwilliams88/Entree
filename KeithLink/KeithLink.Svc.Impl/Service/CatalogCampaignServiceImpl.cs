using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Service
{
    public class CatalogCampaignServiceImpl : ICatalogCampaignService
    {
        #region attributes
        private readonly ICatalogLogic _catalogLogic;
        private readonly ICatalogCampaignLogic _campaignLogic;
        #endregion

        #region constructor
        public CatalogCampaignServiceImpl(ICatalogLogic catalogLogic, ICatalogCampaignLogic campaignLogic)
        {
            _catalogLogic = catalogLogic;
            _campaignLogic = campaignLogic;
        }
        #endregion

        #region functions

        #region get
        public ProductsReturn GetCatalogCampaignProducts(int campaignId, string branch)
        {
            CatalogCampaignReturnModel campaign = _campaignLogic.GetCampaign(campaignId);

            return _catalogLogic.GetProductsByIds(branch, campaign.Items.Select(x => x.ItemNumber).ToList());
        }
        #endregion

        #endregion

    }
}
