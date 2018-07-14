using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Marketing {
    public interface ICatalogCampaignService {
        ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel searchModel, UserProfile profile);
    }
}
