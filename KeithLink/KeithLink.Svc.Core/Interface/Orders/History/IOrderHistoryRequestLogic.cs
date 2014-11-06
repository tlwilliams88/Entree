using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryRequestLogic {
        void ProcessRequests();

        void RequestAllOrdersForCustomer(UserSelectedContext context);

        void RequestOrderForCustomer(UserSelectedContext context, string invoiceNumber); 
    }
}
