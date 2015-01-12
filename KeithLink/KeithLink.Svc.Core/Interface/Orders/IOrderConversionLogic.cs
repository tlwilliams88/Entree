using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IOrderConversionLogic {
        void SaveConfirmationAsOrderHistory(ConfirmationFile confFile);

        void SaveOrderHistoryAsConfirmation(OrderHistoryFile histFile);
    }
}
