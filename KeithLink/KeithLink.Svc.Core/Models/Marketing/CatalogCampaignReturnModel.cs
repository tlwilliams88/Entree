using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing
{
    public class CatalogCampaignReturnModel : CatalogCampaignHeader
    {
        public List<CatalogCampaignItem> Items { get; set; }
    }
}
