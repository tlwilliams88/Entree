using System.Linq;

using Entree.Core.Interface.Marketing;
using Entree.Core.Interface.SiteCatalog;
using Entree.Core.Models.Marketing;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

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
        public ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel model, UserProfile profile){
            CatalogCampaignReturnModel campaign = _campaignLogic.GetCampaignByUri(campaignUri);

            return _catalogLogic.GetProductsByItemNumbers(context, campaign.Items.Select(x => x.ItemNumber).ToList(), model, profile);
        }
        #endregion

    }
}
