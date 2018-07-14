using System;

namespace Entree.Core.Interface.OnlinePayments.Log {
    public interface IKPayLogRepository {
        void Write(string userName, string message);
    }
}
