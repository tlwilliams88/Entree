using Entree.Core.Models.EF;
using Entree.Core.Models.Orders.History.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Orders.History {
    public interface IOrderHistoryDetailRepository : IBaseEFREpository<OrderHistoryDetail> {
    }
}
