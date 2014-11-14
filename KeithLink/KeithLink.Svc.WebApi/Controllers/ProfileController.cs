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
using KeithLink.Svc.WebApi.Attribute;

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
                                                                            userInfo.Role, userInfo.BranchId);
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
                retVal.SuccessResponse = _profileLogic.CreateGuestUserAndProfile(guestInfo.Email, guestInfo.Password, guestInfo.BranchId, base.AuthenticatedUser != null);
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
                return _profileLogic.GetUserProfile(email, true);
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
                if (String.IsNullOrEmpty(userInfo.UserId)) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

                _profileLogic.UpdateUserProfile(userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName, 
                                                userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId);

                UserProfileReturn profile = _profileLogic.GetUserProfile(userInfo.Email);

                // handle customer updates - will need to add security here
                if (userInfo.Customers != null && userInfo.Customers.Count > 0)// && // security here)
                {
                    IEnumerable<Guid> custsToAdd = userInfo.Customers.Select(c =>c.CustomerId).Except(profile.UserProfiles[0].UserCustomers.Select(b => b.CustomerId));
                    IEnumerable<Guid> custsToRemove = profile.UserProfiles[0].UserCustomers.Select(b => b.CustomerId).Except(userInfo.Customers.Select(c => c.CustomerId));
                    foreach (Guid c in custsToAdd)
                        _profileLogic.AddUserToCustomer(c, profile.UserProfiles[0].UserId);
                    foreach (Guid c in custsToRemove)
                        _profileLogic.RemoveUserFromCustomer(c, profile.UserProfiles[0].UserId);
                }

                profile = _profileLogic.GetUserProfile(userInfo.Email);
                retVal.SuccessResponse = profile;
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
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountReturn> CreateAccount(AccountModel account)
        {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.CreateAccount(account.Name);
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
        [HttpPut]
        [ApiKeyedRoute("profile/account")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountReturn> UpdateAccount(AccountModel account)
        {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try
            {
                _profileLogic.UpdateAccount(account.AccountId.Value, account.Name, account.Customers, account.Users);
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
        [ApiKeyedRoute("profile/account")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountReturn> GetAccount(AccountModel account)
        {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetAccount(account.AccountId.Value);
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
        [HttpPut]
        [ApiKeyedRoute("profile/account/user")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<bool> AddUserToAccount(AccountAddUserModel info)
        {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try
            {
                // TODO
                retVal.SuccessResponse = true;
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
        [HttpPut]
        [ApiKeyedRoute("profile/customer/user")]
        public OperationReturnModel<bool> AddUserToCustomer(CustomerAddUserModel info)
        {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try
            {
                _profileLogic.AddUserToCustomer(info.customerId, info.userId);
                retVal.SuccessResponse = true;
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
        //[Authorization(new string[] { Core.Constants.ROLE_EXTERNAL_OWNER })] // TODO - add internal roles
        public OperationReturnModel<CustomerReturn> GetCustomers([FromUri] CustomerFilterModel customerFilter)
        {
            OperationReturnModel<CustomerReturn> retVal = new OperationReturnModel<CustomerReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetCustomers(customerFilter);
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
        [ApiKeyedRoute("profile/accounts")]
        //[Authorization(new string[] { Core.Constants.ROLE_EXTERNAL_OWNER })] // TODO - add internal roles
        public OperationReturnModel<AccountReturn> GetAccounts([FromUri] AccountFilterModel accountFilter)
        {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetAccounts(accountFilter);
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
        [ApiKeyedRoute("profile/users")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<UserProfileReturn> GetUsers([FromUri] UserFilterModel userFilter)
        {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetUsers(userFilter);
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