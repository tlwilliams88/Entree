using KeithLink.Svc.Core.Models.Marketing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignItemRepository
    {
        List<CatalogCampaignItem> GetByCampaign(Int64 campaignId);
        void CreateOrUpdate(Int64 campaignId, CatalogCampaignItem item);
    }
}
