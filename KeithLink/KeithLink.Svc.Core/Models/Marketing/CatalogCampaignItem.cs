using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing
{
    public class CatalogCampaignItem
    {
        public Int64 Id { get; set; }
        public string ItemNumber { get; set; }
        public Int64 CatalogCampaignHeaderId { get; set; }
        public bool Active { get; set; }
    }
}
