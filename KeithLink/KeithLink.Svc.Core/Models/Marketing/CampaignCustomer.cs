using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing {
    public class CampaignCustomer {
        public int CampaignId { get; set; }
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
    }
}
