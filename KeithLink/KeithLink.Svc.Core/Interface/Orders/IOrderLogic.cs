using KeithLink.Svc.Core.Models.Orders;
using System;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
        //void ParseFile(string FileName);

        void SendToHistory(OrderFile order);

        void SendToHost(OrderFile order);
    }
}
