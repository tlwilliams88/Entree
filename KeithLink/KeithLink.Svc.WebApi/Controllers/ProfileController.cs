using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class ProfileController : BaseController
    {
        #region attributes
        private ICustomerContainerRepository    _custRepo;
        private IEventLogRepository             _log;
        private IUserProfileLogic               _profileLogic;
        #endregion

        #region ctor
        public ProfileController(ICustomerContainerRepository customerRepo, 
                                 IEventLogRepository logRepo,
                                 IUserProfileLogic profileLogic) : base(profileLogic) {
            _custRepo = customerRepo;
            _profileLogic = profileLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        [AllowAnonymous]
        [HttpPost]
        [ApiKeyedRoute("profile/create")]
        public OperationReturnModel<UserProfileReturn> CreateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.CreateUserAndProfile(userInfo.CustomerName, userInfo.Email, userInfo.Password,
                                                                            userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber,
                                                                            userInfo.RoleName, userInfo.BranchId);
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        [AllowAnonymous]
        [HttpPost]
        [ApiKeyedRoute("profile/register")]
        public OperationReturnModel<UserProfileReturn> CreateGuest(GuestProfileModel guestInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.CreateGuestUserAndProfile(guestInfo.Email, guestInfo.Password, guestInfo.BranchId);
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile")]
        public UserProfileReturn GetUser(string email) {
            if (string.Compare(email, AuthenticatedUser.EmailAddress, true) == 0) {
                UserProfileReturn retVal = new UserProfileReturn();
                retVal.UserProfiles.Add(this.AuthenticatedUser);

                return retVal;
            } else {
                return _profileLogic.GetUserProfile(email);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ApiKeyedRoute("profile/searchcustomer/{searchText}")]
        public CustomerContainerReturn SearchCustomers(string searchText) {
            return _custRepo.SearchCustomerContainers(searchText);
        }

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/password")]
        public string UpdatePassword(UpdatePasswordModel pwInfo) {
            return _profileLogic.UpdateUserPassword(pwInfo.Email, pwInfo.OriginalPassword, pwInfo.NewPassword);
        }

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile")]
        public OperationReturnModel<UserProfileReturn> UpdateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                if (userInfo.UserId == null || userInfo.UserId.Length == 0) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

                _profileLogic.UpdateUserProfile(userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName, 
                                                userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId);

                retVal.SuccessResponse = _profileLogic.GetUserProfile(userInfo.Email);
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/account")]
        public OperationReturnModel<AccountReturn> CreateAccount(AccountModel account)
        {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try
            {
                _profileLogic.CreateAccount(account.Name);
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/customers")]
        public OperationReturnModel<CustomerReturn> GetAllCustomers()
        {
            OperationReturnModel<CustomerReturn> retVal = new OperationReturnModel<CustomerReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetAllCustomers();
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        #endregion
    }
}