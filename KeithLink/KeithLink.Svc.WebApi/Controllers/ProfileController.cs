using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.WebApi.Models;
using KeithLink.Svc.WebApi.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using KeithLink.Svc.WebApi.Attribute;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class ProfileController : BaseController
    {
        #region attributes
        private IAvatarRepository               _avatarRepository;
        private ICustomerContainerRepository    _custRepo;
        private ICustomerDomainRepository       _extAd;
        private IEventLogRepository             _log;
        private IUserProfileLogic               _profileLogic;
        #endregion

        #region ctor
        public ProfileController(ICustomerContainerRepository customerRepo, 
                                 IEventLogRepository logRepo,
                                 IUserProfileLogic profileLogic,
                                 IAvatarRepository avatarRepository,
                                 ICustomerDomainRepository customerADRepo) : base(profileLogic) {
            _custRepo = customerRepo;
            _profileLogic = profileLogic;
            _log = logRepo;
            _avatarRepository = avatarRepository;
            _extAd = customerADRepo;
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
        [ApiKeyedRoute("profile/register")] // Discussion on route naming
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
        [HttpPost]
        [ApiKeyedRoute( "profile/admin/user" )]
        public OperationReturnModel<UserProfileReturn> CreateGuestWithTemporaryPassword( GuestProfileModel guestInfo ) {
            OperationReturnModel<UserProfileReturn> returnValue = new OperationReturnModel<UserProfileReturn>();

            try {
                returnValue.SuccessResponse = _profileLogic.UserCreatedGuestWithTemporaryPassword( guestInfo.Email, guestInfo.BranchId );
            } catch (ApplicationException ex) {
                returnValue.ErrorMessage = ex.Message;
                _log.WriteErrorLog( "Application exception", ex );
            } catch (Exception ex) {
                returnValue.ErrorMessage = String.Concat( "Could not complete the request. ", ex.Message );
                _log.WriteErrorLog( "Unhandled exception", ex );
            }

            return returnValue;
        }

        [HttpPost]
        [ApiKeyedRoute( "profile/forgotpassword" )]
        public OperationReturnModel<bool> ForgotPassword( string emailAddress ) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

            try {
                _profileLogic.ResetPassword( emailAddress );
                returnValue.SuccessResponse = true;
            } catch (Exception ex) {
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = "There was an error processing your request. Please validate your information is correct. If the problem persists please contact support";
                _log.WriteErrorLog( "Controller reset password error", ex );
            }

            return returnValue;
        }

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile")]
        public UserProfileReturn GetUser(string email) {
            if (string.Compare(email, AuthenticatedUser.EmailAddress, true) == 0) {
                UserProfileReturn retVal = new UserProfileReturn();
                retVal.UserProfiles.Add(this.AuthenticatedUser);
				retVal.UserProfiles.First().DefaultCustomer = _profileLogic.CustomerSearch(this.AuthenticatedUser, string.Empty, new PagingModel() { From = 0, Size = 1 }, string.Empty).Results.FirstOrDefault();
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

		[AllowAnonymous]
		[HttpGet]
		[ApiKeyedRoute("profile/customer/balance")]
		public CustomerBalanceOrderUpdatedModel CustomerBalance()
		{
			return _profileLogic.GetBalanceForCustomer(this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId);
		}

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/password")]
        public OperationReturnModel<bool> UpdatePassword(UpdatePasswordModel pwInfo) {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();
            try {
               returnValue.SuccessResponse = _profileLogic.UpdateUserPassword(pwInfo.Email, pwInfo.OriginalPassword, pwInfo.NewPassword);
            } catch (Exception ex) {
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = ex.Message;
            }

            return returnValue;
        }

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile")]
        public OperationReturnModel<UserProfileReturn> UpdateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                if (String.IsNullOrEmpty(userInfo.UserId)) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

                if (!_profileLogic.IsInternalAddress(userInfo.Email))
                {
                    _profileLogic.UpdateUserProfile(userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName,
                                                  userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId, 
                                                  true /* hard coded security for now */, userInfo.Customers, userInfo.Role);
                }

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
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountReturn> CreateAccount(Account account)
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
        public OperationReturnModel<bool> UpdateAccount(Account account)
        {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
			retVal.SuccessResponse = false;
            try
            {
                retVal.SuccessResponse = _profileLogic.UpdateAccount(account.Id, account.Name, account.Customers, account.AdminUsers);
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
        [ApiKeyedRoute("profile/account/{accountid}")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<Account> GetAccount(Guid accountid)
        {
            OperationReturnModel<Account> retVal = new OperationReturnModel<Account>();

            try
            {
                retVal.SuccessResponse = _profileLogic.GetAccount(accountid);
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
        [HttpGet]
        [ApiKeyedRoute("profile/account/{accountId}/users")]
        //[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
        public OperationReturnModel<AccountUsersReturn> GetAcountUsers(Guid accountId)
        {
            return new OperationReturnModel<AccountUsersReturn>() { SuccessResponse = _profileLogic.GetAccountUsers(accountId) };
        }

		///profile/{userId}/account/{accountId} 
		[Authorize]
		[HttpDelete]
		[ApiKeyedRoute("profile/{userId}/account/{accountId}")]        
		public OperationReturnModel<bool> RemoveUserFromAcocunt(Guid userId, Guid accountId)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				_profileLogic.RemoveUserFromAccount(accountId, userId);
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
		[ApiKeyedRoute("profile/customer/")]
		public PagedResults<Customer> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort, [FromUri] string account = "", [FromUri] string terms = "")
		{
            if (paging.Sort == null && sort != null && !String.IsNullOrEmpty(sort.Order) && !String.IsNullOrEmpty(sort.Field) )
            {
                paging.Sort = new List<SortInfo>() { sort };
            }
			return _profileLogic.CustomerSearch(this.AuthenticatedUser, terms, paging, account);
		}

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/customer/{branchId}/{customerNumber}")]
        public OperationReturnModel<Customer> GetCustomer(string branchId, string customerNumber)
        {
            OperationReturnModel<Customer> retVal = new OperationReturnModel<Customer>();

            try
            {
				retVal.SuccessResponse = _profileLogic.GetCustomerByCustomerNumber(customerNumber, branchId);
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
		[HttpPost]
		[ApiKeyedRoute("profile/accounts")]
		public PagedResults<Account> Accounts(PagingModel paging)
		{
			return _profileLogic.GetPagedAccounts(paging);
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

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile/user/{userid}/customers")]
        public OperationReturnModel<CustomerReturn> GetCustomersForExternalUser(Guid userid) {
            OperationReturnModel<CustomerReturn> customerReturn = new OperationReturnModel<CustomerReturn>();
            try
            {
                customerReturn.SuccessResponse = new CustomerReturn() { Customers = _profileLogic.GetCustomersForExternalUser(userid) };
            }
            catch (Exception ex)
            {
                customerReturn.ErrorMessage = ex.Message;
                _log.WriteErrorLog("Error retrieving customers for external user", ex);
            }

            return customerReturn;
        }


        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/avatar" )]
        public async Task<OperationReturnModel<bool>> UploadAvatar() {
            if (!Request.Content.IsMimeMultipartContent())
				throw new InvalidOperationException();

            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool> { ErrorMessage = null, SuccessResponse = false };

			var provider = new MultipartMemoryStreamProvider();
			await Request.Content.ReadAsMultipartAsync(provider);

            string base64FileString = null;
            string fileName = null;

			foreach (var content in provider.Contents)
			{
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
                returnValue.SuccessResponse = _avatarRepository.SaveAvatar( this.AuthenticatedUser.UserId, fileName, base64FileString );
            } catch (Exception e) {
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = e.Message;
            }

            return returnValue; 
        }

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute( "profile/salesrep" )]
        public OperationReturnModel<bool> GetSalesRep() {
            // Get the DSR
            return new OperationReturnModel<bool>() { SuccessResponse = true };
        }

        [Authorize]
        [HttpPost]
        [ApiKeyedRoute("profile/{email}/access/{appname}")]
        public OperationReturnModel<bool> GrantApplicationAccess(string email, string appname) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                string appRoleName = null;

                switch (appname.ToLower()) {
                    case "kbit":
                        appRoleName = Impl.Configuration.AccessGroupKbitCustomer;
                        break;
                    default:
                        break;
                }

                if (appRoleName == null) {
                    retVal.SuccessResponse = false;
                    retVal.ErrorMessage = "Could not grant access to unknown application.";
                } else {
                    _profileLogic.GrantRoleAccess(email, appRoleName);

                    retVal.SuccessResponse = true;
                }
            } catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.ErrorMessage = "Could grant access";
                _log.WriteErrorLog("Could not grant access to application.", ex);
            }

            return retVal;
        }

        [Authorize]
        [HttpDelete]
        [ApiKeyedRoute("profile/{email}/access/{appname}")]
        public OperationReturnModel<bool> RevokeApplicationAccess(string email, string appname) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                string appRoleName = null;

                switch (appname.ToLower()) {
                    case "kbit":
                        appRoleName = Impl.Configuration.AccessGroupKbitCustomer;
                        break;
                    default:
                        break;
                }

                if (appRoleName == null) {
                    retVal.SuccessResponse = false;
                    retVal.ErrorMessage = "Could not revoke access from unknown application.";
                } else {
                    _profileLogic.RevokeRoleAccess(email, appRoleName);

                    retVal.SuccessResponse = true;
                }
            } catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.ErrorMessage = "Could revoke access";
                _log.WriteErrorLog("Could not revoke access to application.", ex);
            }

            return retVal;
        }
        #endregion
    }
}