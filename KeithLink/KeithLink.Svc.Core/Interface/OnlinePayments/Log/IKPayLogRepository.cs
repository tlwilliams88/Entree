using System;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Log {
    public interface IKPayLogRepository {
        void Write(string userName, string message);
    }
}
