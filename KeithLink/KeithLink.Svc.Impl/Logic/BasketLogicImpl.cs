using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Impl.Logic
{
	public class BasketLogicImpl: IBasketLogic
	{
		private readonly IBasketRepository basketRepository;
		private readonly IUserProfileRepository userProfileRepository;
		private readonly ICustomerRepository customerRepository;

		public BasketLogicImpl(IBasketRepository basketRepository, IUserProfileRepository userProfileRepository, ICustomerRepository customerRepository)
		{
			this.basketRepository = basketRepository;
			this.userProfileRepository = userProfileRepository;
			this.customerRepository = customerRepository;
		}

		public CS.Basket RetrieveSharedCustomerBasket(UserProfile user, UserSelectedContext catalogInfo, Guid listId)
		{
			//Have to find the specific list...try current user first
			var basket = basketRepository.ReadBasket(user.UserId, listId);

			//Was list found?
			if (basket != null)
				return basket;

			//Basket belongs to another user for this customer...find it.
			var sharedUsers = userProfileRepository.GetUsersForCustomerOrAccount(customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId).CustomerId).Where(b => !b.UserId.Equals(user.UserId)).ToList();


			foreach (var sharedUser in sharedUsers)
			{
				var sharedBasket = basketRepository.ReadBasket(sharedUser.UserId, listId);
				if (sharedBasket != null)
					return sharedBasket;
			}

			return null;
		}

		public List<CS.Basket> RetrieveAllSharedCustomerBaskets(UserProfile user, UserSelectedContext catalogInfo, BasketType type, bool includeFavorites = false)
		{
			var sharedUsers = userProfileRepository.GetUsersForCustomerOrAccount(customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId).CustomerId).ToList();

			var returnBaskets = new List<CS.Basket>();

			if (sharedUsers.Any())
			{
				foreach (var sharedUser in sharedUsers)
				{
					var basket = basketRepository.ReadAllBaskets(sharedUser.UserId, type).Where(l => (l.Shared.Equals(true) || l.UserId.Equals(user.UserId.ToCommerceServerFormat()))
						&& !string.IsNullOrEmpty(l.CustomerId) && l.CustomerId.Equals(catalogInfo.CustomerId)).ToList();

					returnBaskets.AddRange(basket);
				}
			}
			else
			{
				var basket = basketRepository.ReadAllBaskets(user.UserId, type).Where(l => (l.Shared.Equals(true) || l.UserId.Equals(user.UserId.ToCommerceServerFormat()))
						&& !string.IsNullOrEmpty(l.CustomerId) && l.CustomerId.Equals(catalogInfo.CustomerId)).ToList();
				returnBaskets.AddRange(basket);
			}

			if (includeFavorites)
			{
				var favorite = basketRepository.ReadAllBaskets(user.UserId, BasketType.Favorite).ToList();
				returnBaskets.AddRange(favorite);
			}

			return returnBaskets;
		}
	}
}
