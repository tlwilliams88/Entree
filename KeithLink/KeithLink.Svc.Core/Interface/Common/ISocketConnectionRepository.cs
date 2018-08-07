namespace KeithLink.Svc.Core.Interface.Common
{
    public interface ISocketConnectionRepository
    {
        void Connect();

        void Close();

        string Receive();

        void Send(string dataRecord);

        void StartTransaction(string transactionId, string confirmationNumber);
    }
}
