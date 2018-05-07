using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;

namespace KeithLink.Svc.Core.Interface.Customers
{
    public interface IGrowthAndRecoveriesRepository {
        List<GrowthAndRecoveriesModel> GetGrowthAdnGetGrowthAndRecoveryOpportunities(string customerNumber, string branchId);
    }
}
