using KeithLink.Svc.Core.Models.Marketing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignHeaderRepository
    {
        CatalogCampaignHeader GetHeader(int id);
        CatalogCampaignHeader GetByUri(string uri);
        List<CatalogCampaignHeader> GetAll();
        long CreateOrUpdate(CatalogCampaignHeader header);
    }
}
