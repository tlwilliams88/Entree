using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IBasketLogic
	{
		CS.Basket RetrieveSharedBasket(UserProfile user, UserSelectedContext catalogInfo, Guid listId);
		List<CS.Basket> RetrieveAllSharedBaskets(UserProfile user, UserSelectedContext catalogInfo, string basketStatus);
		

	}
}
