using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IMainframeRepository
    {
        bool SendOrder(Models.Orders.OrderHeader header, List<Models.Orders.OrderDetail> details);
    }
}
