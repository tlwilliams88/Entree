using System;
using KeithLink.Svc.Core.Models.Marketing;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Marketing
{
    public static class CatalogCampaignHeaderExtensions
    {
        public static CatalogCampaignReturnModel ToModel(this CatalogCampaignHeader from)
        {
            CatalogCampaignReturnModel to = new CatalogCampaignReturnModel();
            to.Id = from.Id;
            to.Description = from.Description;
            to.Active = from.Active;
            to.StartDate = from.StartDate;
            to.EndDate = from.EndDate;

            return to;
        }
    }
}
