using KeithLink.Svc.Core.Interface.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class NoOrderConversionLogicImpl : IOrderConversionLogic {
        #region IOrderConversionLogic Members

        public void SaveConfirmationAsOrderHistory(Core.Models.Orders.Confirmations.ConfirmationFile confFile) {
            throw new NotImplementedException();
        }

        public void SaveOrderHistoryAsConfirmation(Core.Models.Orders.History.OrderHistoryFile histFile) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
