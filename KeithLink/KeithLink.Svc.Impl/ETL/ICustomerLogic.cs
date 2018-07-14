using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.ETL
{
    public interface ICustomerLogic {
        void ImportCustomerItemHistory();
        void ImportCustomersToOrganizationProfile();
        void ImportDsrInfo();
        void ImportUsersWithAccess();
    }
}
