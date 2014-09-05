using System;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
        void ParseFile(string FileName);

        void SendToHistory();

        void SendToHost();
    }
}
