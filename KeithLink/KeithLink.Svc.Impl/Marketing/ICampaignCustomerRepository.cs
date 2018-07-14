using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Marketing;

namespace Entree.Core.Interface.Marketing {
    public interface ICampaignCustomerRepository {
        List<CampaignCustomer> GetAllCustomersByCampaign(long campaignId);
    }
}
