using Entree.Core.Interface.Common;
using Entree.Core.Models.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Orders {
    public interface ISpecialOrderRepository  {
		void Create(OrderFile header);
    }
}
