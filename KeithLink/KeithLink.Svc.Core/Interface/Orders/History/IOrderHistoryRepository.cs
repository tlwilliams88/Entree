using KeithLink.Svc.Core.Models.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IOrderHistoryRepository {
        OrderHistoryFileReturn GetAllOrderHistory(string branchId, string customerNumber);

        OrderHistoryHeaderReturn GetAllOrderHistoryHeaders(string branchId, string customerNumber);

        OrderHistoryFile GetOrderHistory(string branchId, string customerNumber, string invoiceNumber);

        void SaveOrderHistoryFile(OrderHistoryFile file);

		
    }
}
