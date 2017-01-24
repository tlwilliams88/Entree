using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignService
    {
        ProductsReturn GetCatalogCampaignProducts(int campaignId, string branch, SearchInputModel searchModel);
    }
}
