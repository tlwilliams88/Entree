using Entree.Core.Models.Orders.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Orders
{
	public interface IUserActiveCartRepository : IBaseEFREpository<UserActiveCart>
	{
        UserActiveCart GetCurrentUserCart(Guid userId, string branchId, string customerNumber);
	}
}
