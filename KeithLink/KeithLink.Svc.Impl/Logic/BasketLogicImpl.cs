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

			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

			if (customer == null)
				return null;

			return basketRepository.ReadBasket(customer.CustomerId, listId);

		}

		public List<CS.Basket> RetrieveAllSharedCustomerBaskets(UserProfile user, UserSelectedContext catalogInfo, BasketType type, bool includeFavorites = false)
		{

			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

			if (customer == null)
				return new List<CS.Basket>();

			return basketRepository.ReadAllBaskets(customer.CustomerId, type).ToList();
						
		}
	}
}
