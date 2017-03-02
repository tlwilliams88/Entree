using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrder2ListRepository
    {
        Order2List Read(string controlNumber);

        void Write(Order2List o2l);

        void Purge(int PurgeDays);
    }
}
