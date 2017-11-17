using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignService {
        CatalogCampaignsReturnModel GetAllAvailableCampaigns(UserSelectedContext context);
        ProductsReturn GetCatalogCampaignProducts(string campaignUri, UserSelectedContext context, SearchInputModel searchModel, UserProfile profile);
    }
}
