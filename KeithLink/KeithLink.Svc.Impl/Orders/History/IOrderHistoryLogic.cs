using Entree.Core.Models.Orders;
using Entree.Core.Models.Orders.History;
using Entree.Core.Models.Paging;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        string CheckForLostOrders(out string sBody);

        OrderTotalByMonth GetOrderTotalByMonth(UserSelectedContext customerInfo, int numberOfMonths);

        void ListenForMainFrameCalls();

        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);

        void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder);

        string SetLostOrder(string trackingNumber);

        void ListenForQueueMessages();

        void StopListening();

        void SubscribeToQueue();

        void Unsubscribe();

        void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber);
    }
}
