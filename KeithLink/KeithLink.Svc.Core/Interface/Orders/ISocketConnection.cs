using System;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface ISocketConnection
    {
        void Connect();

        void Close();

        string Receive();

        void Send(string dataRecord);

        void StartTransaction(string confirmationNumber);
    }
}
