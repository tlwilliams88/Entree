using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Models.Customers;

namespace Entree.Core.Interface.Customers
{
    public interface IGrowthAndRecoveriesRepository {
        List<GrowthAndRecoveriesModel> GetGrowthAdnGetGrowthAndRecoveryOpportunities(string customerNumber, string branchId);
    }
}
