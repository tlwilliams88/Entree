using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers {
    [Authorize]
    public class CustomerBankController : BaseController {
        #region attributes
        private readonly ICustomerBankServiceRepository _repo;
        #endregion
        
        #region ctor
        public CustomerBankController(IUserProfileLogic profileLogic, ICustomerBankServiceRepository serviceRepo) : base(profileLogic) {
            _repo = serviceRepo;
        }
        #endregion

        #region methods
        [HttpGet]
        [ApiKeyedRoute("banks")]
        public List<CustomerBank> Get() {
            return _repo.GetAllCustomerBanks(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId);
        }

        [HttpGet]
        [ApiKeyedRoute("banks/{accountNumber}")]
        public CustomerBank Get(string accountNumber) {
            return _repo.GetBankAccount(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId, accountNumber);
        }
        #endregion
    }
}