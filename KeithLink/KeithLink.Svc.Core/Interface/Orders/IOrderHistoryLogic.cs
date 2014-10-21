using KeithLink.Svc.Core.Models.Orders.History;
using System;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IOrderHistoryLogic {
        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);
    }
}
