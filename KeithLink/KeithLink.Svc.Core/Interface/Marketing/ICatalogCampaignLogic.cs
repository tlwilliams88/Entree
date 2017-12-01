using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignLogic {
        bool AddOrUpdateCampaign(CatalogCampaignAddOrUpdateRequestModel campaign);
        CatalogCampaignsReturnModel GetAllAvailableCampaigns(UserSelectedContext context);
        CatalogCampaignsReturnModel GetAllCampaigns(bool includeItems = true);
        CatalogCampaignReturnModel GetCampaignByUri(string uri, bool includeItems = true);
        CatalogCampaignReturnModel GetCampaign(int id, bool includeItems = true);
    }
}
