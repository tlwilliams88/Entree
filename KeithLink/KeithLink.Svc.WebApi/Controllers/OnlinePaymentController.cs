using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
	public class OnlinePaymentController : BaseController
	{
		#region attributes
		private readonly IOnlinePaymentServiceRepository _repo;
		#endregion

		#region ctor
		public OnlinePaymentController(IUserProfileLogic profileLogic, IOnlinePaymentServiceRepository serviceRepo)
			: base(profileLogic)
		{
			_repo = serviceRepo;
		}
		#endregion

		#region methods
		/// <summary>
		/// Retrieve customer banks
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("banks")]
		public List<CustomerBank> Get()
		{
			return _repo.GetAllCustomerBanks(this.SelectedUserContext);
		}

		/// <summary>
		/// Retrieve bank information for account
		/// </summary>
		/// <param name="accountNumber">Account number</param>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("banks/{accountNumber}")]
		public CustomerBank GetBackAccount(string accountNumber)
		{
			return _repo.GetBankAccount(this.SelectedUserContext, accountNumber);
		}
		#endregion
	}
}