using Entree.Core.Enumerations.List;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = Entree.Core.Models.Generated;

namespace Entree.Core.Interface.Orders
{
	public interface IBasketLogic
	{
		CS.Basket RetrieveSharedCustomerBasket(UserProfile user, UserSelectedContext catalogInfo, Guid listId);
		List<CS.Basket> RetrieveAllSharedCustomerBaskets(UserProfile user, UserSelectedContext catalogInfo, BasketType type, bool includeFavorites = false);
		

	}
}
