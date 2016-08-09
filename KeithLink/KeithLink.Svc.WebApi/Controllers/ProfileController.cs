﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ModelExport;

using KeithLink.Svc.Impl.Helpers;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// end points for working the user profile
    /// </summary>
	public class ProfileController : BaseController
	{
		#region attributes
		private readonly IAvatarRepository _avatarRepository;
        private readonly ICustomerRepository _custRepo;
		private readonly ICustomerContainerRepository _custContainerRepo;
		private readonly ICustomerDomainRepository _extAd;
		private readonly IEventLogRepository _log;
		private readonly IUserProfileLogic _profileLogic;
		private readonly IPasswordResetLogic _passwordLogic;
        private readonly IDsrAliasLogic _dsrAliasLogic;
		private readonly IMarketingPreferencesLogic _marketingPreferencesLogic;
		private readonly IExportSettingLogic _exportLogic;
        private readonly ISettingsLogic _settingLogic;
		#endregion

		#region ctor
		public ProfileController(ICustomerContainerRepository customerContainerRepo, IEventLogRepository logRepo, IUserProfileLogic profileLogic, 
                                 IAvatarRepository avatarRepository, ICustomerDomainRepository customerADRepo, IPasswordResetLogic passwordResetService, 
                                 IDsrAliasLogic dsrAliasLogic, IMarketingPreferencesLogic marketingPreferencesLogic, IExportSettingLogic exportSettingsLogic, 
                                 ISettingsLogic settingsLogic, ICustomerRepository customerRepository) : base(profileLogic) {
			_custContainerRepo = customerContainerRepo;
            _custRepo = customerRepository;
			_profileLogic = profileLogic;
			_log = logRepo;
			_avatarRepository = avatarRepository;
			_extAd = customerADRepo;
			_passwordLogic = passwordResetService;
            _dsrAliasLogic = dsrAliasLogic;
			_marketingPreferencesLogic = marketingPreferencesLogic;
			_exportLogic = exportSettingsLogic;
            _settingLogic = settingsLogic;

        }
		#endregion

        #region methods
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="userInfo">User</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/create")]
        public OperationReturnModel<UserProfileReturn> CreateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.CreateUserAndProfile(this.AuthenticatedUser, userInfo.CustomerName, userInfo.Email, userInfo.Password,
                                                                            userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber,
                                                                            userInfo.Role, userInfo.BranchId);
                retVal.IsSuccess = true;
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Register as guest
        /// </summary>
        /// <param name="guestInfo">Guest</param>
        /// <returns></returns>
        /// <remarks>
        /// jwames - 5/19/2015 - handle the auditing of registering a new user
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        [ApiKeyedRoute("profile/register")] // Discussion on route naming
        public OperationReturnModel<UserProfileReturn> CreateGuest(GuestProfileModel guestInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                if (this.AuthenticatedUser == null) {
                    // fake a user profile for registration
                    UserProfile tempUser = new UserProfile() { EmailAddress = "registration" };
                    retVal.SuccessResponse = _profileLogic.CreateGuestUserAndProfile(tempUser, guestInfo.Email, guestInfo.Password, guestInfo.BranchId);
                } else {
                    retVal.SuccessResponse = _profileLogic.CreateGuestUserAndProfile(this.AuthenticatedUser, guestInfo.Email, guestInfo.Password, guestInfo.BranchId);
                }

                MarketingPreferenceModel model = new MarketingPreferenceModel() {
                    Email = guestInfo.Email,
                    BranchId = guestInfo.BranchId,
                    CurrentCustomer = guestInfo.ExistingCustomer,
                    LearnMore = guestInfo.MarketingFlag,
                    RegisteredOn = DateTime.Now
                };

                _marketingPreferencesLogic.CreateMarketingPreference(model);
                _settingLogic.SetDefaultApplicationSettings(guestInfo.Email);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Create guest and assign a temporary password
        /// </summary>
        /// <param name="guestInfo">Guest</param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/admin/user")]
        public OperationReturnModel<UserProfileReturn> CreateGuestWithTemporaryPassword(GuestProfileModel guestInfo) {
            OperationReturnModel<UserProfileReturn> returnValue = new OperationReturnModel<UserProfileReturn>();

            try {
                returnValue.SuccessResponse = _profileLogic.UserCreatedGuestWithTemporaryPassword(this.AuthenticatedUser, guestInfo.Email, guestInfo.BranchId);
                _settingLogic.SetDefaultApplicationSettings(guestInfo.Email);
                returnValue.IsSuccess = true;
            }
            catch (ApplicationException ex) {
                returnValue.ErrorMessage = ex.Message;
                returnValue.IsSuccess = false;
                _log.WriteErrorLog("Application exception", ex);
            } catch (Exception ex) {
                returnValue.ErrorMessage = String.Concat("Could not complete the request. ", ex.Message);
                returnValue.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile")]
        public Models.OperationReturnModel<UserProfileReturn> GetUser(string email = "") {
            Models.OperationReturnModel<UserProfileReturn> retVal = new Models.OperationReturnModel<UserProfileReturn>();
            try
            {
                if (email == string.Empty || string.Compare(email, AuthenticatedUser.EmailAddress, true) == 0)
                {
                    UserProfileReturn upVal = new UserProfileReturn();
                    upVal.UserProfiles.Add(this.AuthenticatedUser);

                    PagedResults<Customer> customers = _profileLogic.CustomerSearch(this.AuthenticatedUser, string.Empty, new PagingModel() { From = 0, Size = 1 }, string.Empty, CustomerSearchType.Customer);

                    if (customers.Results == null)
                    {
                        upVal.UserProfiles.First().DefaultCustomer = null;
                    }
                    else {
                        upVal.UserProfiles.First().DefaultCustomer = customers.Results.FirstOrDefault();
                    }

                    // UNFI Whitelisting configurations - these are temporary entries
                    // if user can view unfi, we want to say this right away
                    if ((upVal.UserProfiles != null) &&
                        (upVal.UserProfiles.Count == 1) &&
                        (upVal.UserProfiles[0].DefaultCustomer != null))
                    {
                        upVal.UserProfiles[0].DefaultCustomer.CanViewUNFI = _profileLogic.CheckCanViewUNFI(this.AuthenticatedUser, 
                            upVal.UserProfiles[0].DefaultCustomer.CustomerNumber, upVal.UserProfiles[0].DefaultCustomer.CustomerBranch);
                    }

                    retVal.SuccessResponse = upVal;
                }
                else {
                    retVal.SuccessResponse = _profileLogic.GetUserProfile(email, true);
                }
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetUser", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve customers
        /// </summary>
        /// <param name="searchText">Search text</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ApiKeyedRoute("profile/searchcustomer/{searchText}")]
        public Models.OperationReturnModel<CustomerContainerReturn> SearchCustomers(string searchText) {
            Models.OperationReturnModel<CustomerContainerReturn> retVal = new Models.OperationReturnModel<CustomerContainerReturn>();
            try
            {
                retVal.SuccessResponse = _custContainerRepo.SearchCustomerContainers(searchText);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("SearchCustomers", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve balance for customer
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ApiKeyedRoute("profile/customer/balance")]
        public Models.OperationReturnModel<CustomerBalanceOrderUpdatedModel> CustomerBalance() {
            Models.OperationReturnModel<CustomerBalanceOrderUpdatedModel> retVal = new Models.OperationReturnModel<CustomerBalanceOrderUpdatedModel>();
            try
            {
                retVal.SuccessResponse = _profileLogic.GetBalanceForCustomer(this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("CustomerBalance", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="pwInfo">Password information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/password")]
        public OperationReturnModel<bool> UpdatePassword(UpdatePasswordModel pwInfo) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();
            try {
                returnValue.SuccessResponse = _profileLogic.UpdateUserPassword(this.AuthenticatedUser, pwInfo.Email, pwInfo.OriginalPassword, pwInfo.NewPassword);
                returnValue.IsSuccess = true;
            }
            catch (Exception ex) {
                _log.WriteErrorLog("UpdatePassword", ex);
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = ex.Message;
                returnValue.IsSuccess = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userInfo">User</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile")]
        public OperationReturnModel<UserProfileReturn> UpdateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                if (String.IsNullOrEmpty(userInfo.UserId)) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

                if (!ProfileHelper.IsInternalAddress(userInfo.Email)) {
                    _profileLogic.UpdateUserProfile(this.AuthenticatedUser, userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName,
                                                  userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId,
                                                  true /* hard coded security for now */, userInfo.Customers, userInfo.Role);
                }

                retVal.SuccessResponse = _profileLogic.GetUserProfile(userInfo.Email);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Create account
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/account")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountReturn> CreateAccount(Account account) {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.CreateAccount(this.AuthenticatedUser, account.Name);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Delete account (customer group)
        /// </summary>
        /// <param name="accountId">Id for account to delete</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [ApiKeyedRoute("profile/account/{accountId}")]
        public OperationReturnModel<bool> DeleteAccount(Guid accountId) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _profileLogic.DeleteAccount(this.AuthenticatedUser, accountId);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.SuccessResponse = false;
                _log.WriteErrorLog("Application exception", axe);
                retVal.IsSuccess = false;
            }
            catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.SuccessResponse = false;
                _log.WriteErrorLog("Unhandled exception", ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }


        /// <summary>
        /// Update account
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/account")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<bool> UpdateAccount(Account account) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
            retVal.SuccessResponse = false;
            try {
                retVal.SuccessResponse = _profileLogic.UpdateAccount(this.AuthenticatedUser, account.Id, account.Name, account.Customers, account.AdminUsers);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve account
        /// </summary>
        /// <param name="accountid">Account id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/account/{accountid}")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<Account> GetAccount(Guid accountid) {
            OperationReturnModel<Account> retVal = new OperationReturnModel<Account>();

            try {
                retVal.SuccessResponse = _profileLogic.GetAccount(this.AuthenticatedUser, accountid);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Add user to account
        /// </summary>
        /// <param name="info">Account and user</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/account/user")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<bool> AddUserToAccount(AccountAddUserModel info) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                // TODO
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve all users for an account
        /// </summary>
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/account/{accountId}/users")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountUsersReturn> GetAcountUsers(Guid accountId) {
            OperationReturnModel<AccountUsersReturn> retVal = new OperationReturnModel<AccountUsersReturn>();
            try
            {
                retVal.SuccessResponse = _profileLogic.GetAccountUsers(accountId);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Remove user from an account
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="accountId">Account id</param>
        /// <returns></returns> 
        [Authorize]
        [HttpDelete]
        [ApiKeyedRoute("profile/{userId}/account/{accountId}")]
        public OperationReturnModel<bool> RemoveUserFromAcocunt(Guid userId, Guid accountId) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _profileLogic.RemoveUserFromAccount(this.AuthenticatedUser, accountId, userId);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Add user to a customer
        /// </summary>
        /// <param name="info">Customer and user</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/customer/user")]
        public OperationReturnModel<bool> AddUserToCustomer(CustomerAddUserModel info) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _profileLogic.AddUserToCustomer(this.AuthenticatedUser, info.customerId, info.userId);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Paged search of customers
        /// </summary>
        /// <param name="paging">Paging information</param>
        /// <param name="sort">Sort object</param>
        /// <param name="account">Account</param>
        /// <param name="terms">Search text</param>
        /// <param name="type">The type of text we are searching for. Is converted to CustomerSearchType enumerator</param>
        /// <returns>search results as a paged list of customers</returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/customer/")]
        public OperationReturnModel<PagedResults<Customer>> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort, [FromUri] string account = "",
                                                                                    [FromUri] string terms = "", [FromUri] string type = "1") {
            OperationReturnModel<PagedResults<Customer>> retVal = new OperationReturnModel<PagedResults<Customer>>();

            try
            {
                if (paging.Sort == null && sort != null && !String.IsNullOrEmpty(sort.Order) && !String.IsNullOrEmpty(sort.Field))
                {
                    paging.Sort = new List<SortInfo>() { sort };
                }

                int typeVal;
                if (!int.TryParse(type, out typeVal))
                {
                    typeVal = 1;
                }

                retVal.SuccessResponse = _profileLogic.CustomerSearch(this.AuthenticatedUser, terms, paging, account, (CustomerSearchType)typeVal);

                // Set the customers UNFI viewing capabilities
                retVal.SuccessResponse.Results.ForEach(x => x.CanViewUNFI = _profileLogic.CheckCanViewUNFI(this.AuthenticatedUser, x.CustomerNumber, x.CustomerBranch));

                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <param name="branchId">Branch id</param>
        /// <param name="customerNumber">Customer number</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/customer/{branchId}/{customerNumber}")]
        public OperationReturnModel<Customer> GetCustomer(string branchId, string customerNumber) {
            OperationReturnModel<Customer> retVal = new OperationReturnModel<Customer>();

            try {
                retVal.SuccessResponse = _profileLogic.GetCustomerByCustomerNumber(customerNumber, branchId);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve accounts
        /// </summary>
        /// <param name="accountFilter">Filter/search information</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/accounts")]
        //[Authorization(new string[] { Core.Constants.ROLE_EXTERNAL_OWNER })] // TODO - add internal roles
        public OperationReturnModel<AccountReturn> GetAccounts([FromUri] AccountFilterModel accountFilter) {
            OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.GetAccounts(accountFilter);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve paged list of accounts
        /// </summary>
        /// <param name="paging">Paging information</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/accounts")]
        public OperationReturnModel<PagedResults<Account>> Accounts(PagingModel paging) {
            OperationReturnModel<PagedResults<Account>> retVal = new OperationReturnModel<PagedResults<Account>>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetPagedAccounts(paging);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve users
        /// </summary>
        /// <param name="userFilter"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/users")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<UserProfileReturn> GetUsers([FromUri] UserFilterModel userFilter) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                retVal.SuccessResponse = _profileLogic.GetUsers(userFilter);
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve all users for external user (Is this still used???)
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/user/{userid}/customers")]
        public OperationReturnModel<CustomerReturn> GetCustomersForExternalUser(Guid userid) {
            OperationReturnModel<CustomerReturn> customerReturn = new OperationReturnModel<CustomerReturn>();
            try {
                customerReturn.SuccessResponse = new CustomerReturn() { Customers = _profileLogic.GetCustomersForExternalUser(userid) };
                customerReturn.IsSuccess = true;
            }
            catch (Exception ex) {
                customerReturn.ErrorMessage = ex.Message;
                _log.WriteErrorLog("Error retrieving customers for external user", ex);
                customerReturn.IsSuccess = false;
            }

            return customerReturn;
        }

        /// <summary>
        /// Upload user avatar
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/avatar")]
        public async Task<OperationReturnModel<bool>> UploadAvatar() {
            if (!Request.Content.IsMimeMultipartContent())
                throw new InvalidOperationException();

            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool> { ErrorMessage = null, SuccessResponse = false };

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            string base64FileString = null;
            string fileName = null;

            foreach (var content in provider.Contents) {
                var data = content;
                var paramName = data.Headers.ContentDisposition.Name.Trim('\"');
                var buffer = await data.ReadAsByteArrayAsync();
                var stream = new System.IO.MemoryStream(buffer);

                if (paramName.Equals("file")) {
                    base64FileString = Convert.ToBase64String(buffer, Base64FormattingOptions.None);
                }

                if (paramName.Equals("name")) {
                    using (var s = new System.IO.StreamReader(stream)) {
                        fileName = s.ReadToEnd();
                    }
                }
            }

            try {
                returnValue.SuccessResponse = _avatarRepository.SaveAvatar(this.AuthenticatedUser.UserId, fileName, base64FileString);
                returnValue.IsSuccess = true;
            }
            catch (Exception e) {
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = e.Message;
                _log.WriteErrorLog("UploadAvatar", e);
                returnValue.IsSuccess = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Grant account to external applications(kbit, powermenu)
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="appname">Application</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/{email}/access/{appname}")]
        public OperationReturnModel<bool> GrantApplicationAccess(string email, string appname) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                AccessRequestType selectedApp = AccessRequestType.Undefined;

                switch (appname.ToLower()) {
                    case "kbit":
                        selectedApp = AccessRequestType.KbitCustomer;
                        break;
                    case "powermenu":
                        selectedApp = AccessRequestType.PowerMenu;
                        break;
                    default:
                        break;
                }

                if (selectedApp == AccessRequestType.Undefined) {
                    retVal.SuccessResponse = false;
                    retVal.IsSuccess = false;
                    retVal.ErrorMessage = "Could not grant access to unknown application.";
                } else {
                    _profileLogic.GrantRoleAccess(this.AuthenticatedUser, email, selectedApp);
                    retVal.IsSuccess = true;
                    retVal.SuccessResponse = true;
                }
            } catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.IsSuccess = false;
                retVal.ErrorMessage = "Could not grant access";
                _log.WriteErrorLog("Could not grant access to application.", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Remove user acces to external application
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="appname">Application</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [ApiKeyedRoute("profile/{email}/access/{appname}")]
        public OperationReturnModel<bool> RevokeApplicationAccess(string email, string appname) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                AccessRequestType selectedApp = AccessRequestType.Undefined;

                switch (appname.ToLower()) {
                    case "kbit":
                        selectedApp = AccessRequestType.KbitCustomer;
                        break;
                    case "powermenu":
                        selectedApp = AccessRequestType.PowerMenu;
                        break;
                    default:
                        break;
                }

                if (selectedApp == AccessRequestType.Undefined) {
                    retVal.SuccessResponse = false;
                    retVal.IsSuccess = false;
                    retVal.ErrorMessage = "Could not revoke access from unknown application.";
                } else {
                    _profileLogic.RevokeRoleAccess(this.AuthenticatedUser, email, selectedApp);
                    retVal.IsSuccess = true;
                    retVal.SuccessResponse = true;
                }
            } catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.IsSuccess = false;
                retVal.ErrorMessage = "Could revoke access";
                _log.WriteErrorLog("Could not revoke access to application.", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Generate forgot password email
        /// </summary>
        /// <param name="emailAddress">Email</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/forgotpassword")]
        public OperationReturnModel<bool> ForgotPassword(string emailAddress) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

            try {
                _passwordLogic.GeneratePasswordResetLink(emailAddress);
                returnValue.IsSuccess = true;
                returnValue.SuccessResponse = true;
            } catch (Exception ex) {
                returnValue.SuccessResponse = false;
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = "There was an error processing your request. Please validate your information is correct. If the problem persists please contact support";
                _log.WriteErrorLog("Controller reset password error", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// Validate user reset password token
        /// </summary>
        /// <param name="tokenModel">Token information</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/forgotpassword/validatetoken")]
        public Models.OperationReturnModel<string> ValidateToken(ValidateTokenModel tokenModel) {
            Models.OperationReturnModel<string> retVal = new Models.OperationReturnModel<string>();
            try
            {
                retVal.SuccessResponse = _passwordLogic.IsTokenValid(tokenModel.Token);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ValidateToken", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Change user password, using the forgot password token
        /// </summary>
        /// <param name="resetModel">Password change information</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/forgotpassword/change")]
        public OperationReturnModel<bool> ChangeForgotPassword(ResetPasswordModel resetModel) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

            try {
                returnValue.SuccessResponse = _passwordLogic.ResetPassword(resetModel);
                returnValue.IsSuccess = true;
            }
            catch (Exception ex) {
                returnValue.SuccessResponse = false;
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = "There was an error processing your request. If the problem persists please contact support";
                _log.WriteErrorLog("Reset password error", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// create a new alias for a user
        /// </summary>
        /// <param name="model">needs the user id, email, branch, and dsr number</param>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// jwames - 4/30/2015 - original code
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/dsralias")]
        public OperationReturnModel<DsrAliasModel> CreateDsrAlias(DsrAliasModel model) {
            OperationReturnModel<DsrAliasModel> retVal = new OperationReturnModel<DsrAliasModel>();

            try {
                retVal.SuccessResponse = _profileLogic.CreateDsrAlias(model.UserId, model.Email, new Dsr() { Branch = model.BranchId, DsrNumber = model.DsrNumber });
                retVal.IsSuccess = true;
            }
            catch (Exception ex) {
                retVal.ErrorMessage = "Could not create alias";
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Could not create alias", ex);
            }

            return retVal;
        }

        /// <summary>
        /// delete the dsr alias
        /// </summary>
        /// <param name="model">the dsr alias id and the email address are required</param>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// jwames - 4/30/2015 - original code
        /// </remarks>
        [Authorize]
        [HttpDelete]
        [ApiKeyedRoute("profile/dsralias")]
        public OperationReturnModel<bool> DeleteDsrAlias(DsrAliasModel model) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _profileLogic.DeleteDsrAlias(model.Id, model.Email);
                retVal.IsSuccess = true;
                retVal.SuccessResponse = true;
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not delete alias";
                _log.WriteErrorLog("Could not delete alias", ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// return all of the DSR aliases for the current user
        /// </summary>
        /// <returns>list of dsr aliases</returns>
        /// <remarks>
        /// jwames - 4/30/2015 - original code
        /// </remarks>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/dsralias")]
        public OperationReturnModel<List<DsrAliasModel>> GetDsrAliases() {
            OperationReturnModel<List<DsrAliasModel>> retVal = new OperationReturnModel<List<DsrAliasModel>>();

            try {
                retVal.SuccessResponse = _profileLogic.GetAllDsrAliasesByUserId(AuthenticatedUser.UserId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex) {
                retVal.ErrorMessage = "Could not get aliases for current user";
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// return all of the DSR Aliases for the speicified user
        /// </summary>
        /// <param name="userId">the user's id</param>
        /// <returns>list of dsr aliases</returns>
        /// <remarks>
        /// jwames - 4/30/2015 - original code
        /// </remarks>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/dsralias/{userId}")]
        public OperationReturnModel<List<DsrAliasModel>> GetDsrAliases(Guid userId) {
            OperationReturnModel<List<DsrAliasModel>> retVal = new OperationReturnModel<List<DsrAliasModel>>();

            try {
                retVal.SuccessResponse = _profileLogic.GetAllDsrAliasesByUserId(userId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex) {
                retVal.ErrorMessage = string.Format("Could not get aliases for speicified user {0}", userId);
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">Registered from date</param>
        /// <param name="to">Registered to date</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/marketinginfo")]
        public OperationReturnModel<List<MarketingPreferenceModel>> GetMarketingInfo([FromUri] DateTime from, [FromUri] DateTime to) {
            OperationReturnModel<List<MarketingPreferenceModel>> retVal = new OperationReturnModel<List<MarketingPreferenceModel>>();

            try
            {
                retVal.SuccessResponse = _marketingPreferencesLogic.ReadMarketingPreferences(from, to);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        // <summary>
        /// Export marketing info to CSV, TAB, or Excel
        /// </summary>
        /// <param name="from">Registered from date</param>
        /// <param name="to">Registered to date</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/export/marketinginfo")]
        public HttpResponseMessage ExportMarketingPrefs([FromUri] DateTime from, [FromUri] DateTime to, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                var marketinginfo = _marketingPreferencesLogic.ReadMarketingPreferences(from, to);

                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.MarketingPreferences, Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
                return ExportModel<MarketingPreferenceModel>(marketinginfo, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportMarketingPrefs", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for marketing prefs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("profile/export/marketinginfo")]
        public OperationReturnModel<ExportOptionsModel> ExportMarketingPref() {
            OperationReturnModel<ExportOptionsModel> retVal = new OperationReturnModel<ExportOptionsModel>();

            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions
                    (this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.MarketingPreferences, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Get a list of settings for a user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("profile/settings")]
        public OperationReturnModel<List<SettingsModelReturn>> GetProfileSettings() {
            OperationReturnModel<List<SettingsModelReturn>> returnValue = new OperationReturnModel<List<SettingsModelReturn>>() { SuccessResponse = null };

            try {
                returnValue.SuccessResponse = _settingLogic.GetAllUserSettings(AuthenticatedUser.UserId);
                returnValue.IsSuccess = true;
            }
            catch (Exception ex) {
                returnValue.ErrorMessage = ex.Message;
                _log.WriteErrorLog(returnValue.ErrorMessage, ex);
                returnValue.IsSuccess = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Create or update profile settings
        /// </summary>
        /// <param name="settings">settings object</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/settings")]
        public OperationReturnModel<bool> CreateOrUpdateProfileSettings(SettingsModel settings) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>() { SuccessResponse = false };

            // Set the userid
            settings.UserId = AuthenticatedUser.UserId;

            try {
                _settingLogic.CreateOrUpdateSettings(settings);
                returnValue.SuccessResponse = true;
                returnValue.IsSuccess = true;
            }
            catch (Exception ex) {
                returnValue.ErrorMessage = string.Format("Error saving profile settings for user: {0}", ex);
                _log.WriteErrorLog(returnValue.ErrorMessage, ex);
                returnValue.IsSuccess = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Delete profile settings
        /// </summary>
        /// <param name="settings">settings object</param>
        /// <returns></returns>
	    [HttpDelete]
	    [ApiKeyedRoute("profile/settings")]
	    public OperationReturnModel<bool> DeleteProfileSettings(SettingsModel settings) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>() { SuccessResponse = false };

            settings.UserId = AuthenticatedUser.UserId;

            try {
                _settingLogic.DeleteSettings(settings);
                returnValue.SuccessResponse = true;
                returnValue.IsSuccess = true;
            }
            catch (Exception ex) {
                returnValue.ErrorMessage = String.Format("Error deleting profile settings {0} for userId {1}", settings.Key, settings.UserId);
                _log.WriteErrorLog(returnValue.ErrorMessage, ex);
                returnValue.IsSuccess = false;
            }

            return returnValue;
        }

        /// <summary>
        /// changes wheter or not a customer can view pricing
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("profile/customer/viewpricing")]
        public OperationReturnModel<bool> UpdateCustomerCanViewPricing(Customer customer) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _custRepo.UpdateCustomerCanViewPricing(customer.CustomerId, customer.CanViewPricing);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.SuccessResponse = false;
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
                retVal.IsSuccess = false;
            }

            return retVal;
        }
        #endregion
    }
}