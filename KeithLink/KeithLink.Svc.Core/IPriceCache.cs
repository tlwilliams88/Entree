using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    interface IPriceCache
    {
        void AddItem(string branchId, string customerNumber, string itemNumber, double casePrice, double packagePrice);

        void ResetAllItems();

        void ResetItemsByCustomer(string branchId, string customerNumber);

        void RemoveItem(string branchId, string customerNumber, string itemNumber);
    }
}
