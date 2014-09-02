using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class MainframeRepositoryImpl : KeithLink.Svc.Core.Interface.Orders.IMainframeRepository
    {
        #region

        public bool SendOrder(Core.Models.Orders.OrderHeader header, List<Core.Models.Orders.OrderDetail> details)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
