using Entree.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Orders
{
    public interface IOrderedItemsFromListRepository
    {
        OrderedItemFromList Read(string controlNumber, string ItemNumber);

        void Write(OrderedItemFromList orderedItemFromList);

        void Purge(int PurgeDays);
    }
}
