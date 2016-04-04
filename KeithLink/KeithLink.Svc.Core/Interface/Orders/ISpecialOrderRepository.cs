using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface ISpecialOrderRepository  {
		void Create(OrderFile header);
    }
}
