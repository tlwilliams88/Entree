using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing
{
    public class CatalogCampaignsReturnModel {
        #region constructor
        public CatalogCampaignsReturnModel() {
            campaigns = new List<CatalogCampaignReturnModel>();
        }
        #endregion

        #region properties
        public List<CatalogCampaignReturnModel> campaigns { get; set; }
        #endregion
    }
}
