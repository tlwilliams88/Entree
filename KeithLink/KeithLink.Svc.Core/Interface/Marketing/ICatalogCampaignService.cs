using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Marketing {
    public interface ICatalogCampaignService {
        ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel searchModel, UserProfile profile);
    }
}
