using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.IO;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        string CheckForLostOrders(out string sBody);

        OrderTotalByMonth GetOrderTotalByMonth(UserSelectedContext customerInfo, int numberOfMonths);

        void ListenForMainFrameCalls();

        void ListenForQueueMessages();

        void ProcessOrder(string rawOrder);

        OrderHistoryFileReturn ParseMainframeFile(StreamReader reader);

        string ReadOrderFromQueue();

        void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder);

        string SetLostOrder(string trackingNumber);

        void StopListening();

        void SubscribeToQueue();

        void Unsubscribe();

        void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber);
    }
}
