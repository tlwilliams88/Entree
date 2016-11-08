// KeithLink
using KeithLink.Svc.Core.Models.EF;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface ICustomInventoryItemsRepository {
        // Retrieve
        CustomInventoryItem Get(long id);
        CustomInventoryItem Get(string branchId, string customerNumber, string itemNumber);
        List<CustomInventoryItem> GetItemsByItemIds(List<long> items);
        List<CustomInventoryItem> GetItemsByBranchAndCustomer(string branchId, string customerNumber);

        // Save
        void Save(CustomInventoryItem item);
        void SaveRange(List<CustomInventoryItem> items);

        // Deletes
        void Delete(long id);
        void DeleteRange(List<CustomInventoryItem> items);

    }
}
