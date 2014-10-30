using KeithLink.Svc.Core.Models.Orders.History;
using System;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        void ListenForQueueMessages();

        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);

        void Save(OrderHistoryFile currentFile);

        void StopListening();
    }
}
