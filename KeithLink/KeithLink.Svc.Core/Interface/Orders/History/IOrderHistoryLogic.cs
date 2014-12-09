using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        List<Order> GetOrders(UserSelectedContext customerInfo);

        void ListenForMainFrameCalls();

        void ListenForQueueMessages();

        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);

        void Save(OrderHistoryFile currentFile);

        void StopListening();
    }
}
