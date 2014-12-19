﻿using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        List<Order> GetOrders(Guid userId, UserSelectedContext customerInfo);

        void ListenForMainFrameCalls();

        void ListenForQueueMessages();

        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);

        Order ReadOrder(string branchId, string orderNumber);

        void Save(OrderHistoryFile currentFile);

        void StopListening();

		List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);
    }
}
