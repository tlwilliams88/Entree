using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing {
    public class CatalogCampaignReturnModel : CatalogCampaignHeader {
        [DataMember(Name="image_desktop")]
        public string ImageDesktop { get; set; }
        [DataMember(Name="image_mobile")]
        public string ImageMobile { get; set; }
    }
}
