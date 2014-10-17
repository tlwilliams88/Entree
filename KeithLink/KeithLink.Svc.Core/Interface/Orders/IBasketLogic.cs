using KeithLink.Svc.Core.Enumerations.List;
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
		CS.Basket RetrieveSharedCustomerBasket(UserProfile user, UserSelectedContext catalogInfo, Guid listId);
		List<CS.Basket> RetrieveAllSharedCustomerBaskets(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool includeFavorites = false);
		

	}
}
