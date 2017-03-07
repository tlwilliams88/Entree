using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderedFromListRepository
    {
        OrderedFromList Read(string controlNumber);

        void Write(OrderedFromList o2l);

        void Delete(string controlNumber);

        void Purge(int PurgeDays);
    }
}
