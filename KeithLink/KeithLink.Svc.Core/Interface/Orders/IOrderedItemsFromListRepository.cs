using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderedItemsFromListRepository
    {
        OrderItemFromList Read(string controlNumber, string ItemNumber);

        void Write(OrderItemFromList o2l);

        void Purge(int PurgeDays);
    }
}
