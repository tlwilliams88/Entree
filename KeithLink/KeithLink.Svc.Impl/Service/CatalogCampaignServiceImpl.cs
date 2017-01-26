using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.Profile;
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
        public ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel model, UserProfile profile)
        {
            CatalogCampaignReturnModel campaign = _campaignLogic.GetCampaignByUri(campaignUri);

            return _catalogLogic.GetProductsByItemNumbers(context, campaign.Items.Select(x => x.ItemNumber).ToList(), model, profile);
        }

        #endregion

        #endregion

    }
}
