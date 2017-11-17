using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Interface.Marketing {
    public interface ICampaignCustomerRepository {
        List<CampaignCustomer> GetAllCustomersByCampaign(int campaignId);
    }
}
