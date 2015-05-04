using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
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
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;

namespace KeithLink.Svc.WebApi.Controllers
{
	public class ProfileController : BaseController
	{
		#region attributes
		private IAvatarRepository _avatarRepository;
		private ICustomerContainerRepository _custRepo;
		private ICustomerDomainRepository _extAd;
		private IEventLogRepository _log;
		private IUserProfileLogic _profileLogic;
		private readonly IPasswordResetService _passwordResetService;
        private readonly IDsrAliasService _dsrAliasService;
		#endregion

		#region ctor
		public ProfileController(ICustomerContainerRepository customerRepo,
								 IEventLogRepository logRepo,
								 IUserProfileLogic profileLogic,
								 IAvatarRepository avatarRepository,
								 ICustomerDomainRepository customerADRepo,
			                     IPasswordResetService passwordResetService,
                                 IDsrAliasService dsrAliasService)
			: base(profileLogic)
		{
			_custRepo = customerRepo;
			_profileLogic = profileLogic;
			_log = logRepo;
			_avatarRepository = avatarRepository;
			_extAd = customerADRepo;
			_passwordResetService = passwordResetService;
            _dsrAliasService = dsrAliasService;
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
		public OperationReturnModel<UserProfileReturn> CreateUser(UserProfileModel userInfo)
		{
			OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

			try
			{
				retVal.SuccessResponse = _profileLogic.CreateUserAndProfile(this.AuthenticatedUser, userInfo.CustomerName, userInfo.Email, userInfo.Password,
																			userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber,
																			userInfo.Role, userInfo.BranchId);
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

		/// <summary>
		/// Register as guest
		/// </summary>
		/// <param name="guestInfo">Guest</param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost]
		[ApiKeyedRoute("profile/register")] // Discussion on route naming
		public OperationReturnModel<UserProfileReturn> CreateGuest(GuestProfileModel guestInfo)
		{
			OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

			try
			{
				retVal.SuccessResponse = _profileLogic.CreateGuestUserAndProfile(this.AuthenticatedUser, guestInfo.Email, guestInfo.Password, guestInfo.BranchId);
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

		/// <summary>
		/// Create guest and assign a temporary password
		/// </summary>
		/// <param name="guestInfo">Guest</param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[ApiKeyedRoute("profile/admin/user")]
		public OperationReturnModel<UserProfileReturn> CreateGuestWithTemporaryPassword(GuestProfileModel guestInfo)
		{
			OperationReturnModel<UserProfileReturn> returnValue = new OperationReturnModel<UserProfileReturn>();

			try
			{
				returnValue.SuccessResponse = _profileLogic.UserCreatedGuestWithTemporaryPassword(this.AuthenticatedUser, guestInfo.Email, guestInfo.BranchId);
			}
			catch (ApplicationException ex)
			{
				returnValue.ErrorMessage = ex.Message;
				_log.WriteErrorLog("Application exception", ex);
			}
			catch (Exception ex)
			{
				returnValue.ErrorMessage = String.Concat("Could not complete the request. ", ex.Message);
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
		public UserProfileReturn GetUser(string email = "")
		{
			if (email == string.Empty || string.Compare(email, AuthenticatedUser.EmailAddress, true) == 0)
			{
				UserProfileReturn retVal = new UserProfileReturn();
				retVal.UserProfiles.Add(this.AuthenticatedUser);
				retVal.UserProfiles.First().DefaultCustomer = _profileLogic.CustomerSearch(this.AuthenticatedUser, string.Empty, new PagingModel() { From = 0, Size = 1 }, string.Empty).Results.FirstOrDefault();
				return retVal;
			}
			else
			{
				return _profileLogic.GetUserProfile(email, true);
			}
		}

		/// <summary>
		/// Retrieve customers
		/// </summary>
		/// <param name="searchText">Search text</param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpGet]
		[ApiKeyedRoute("profile/searchcustomer/{searchText}")]
		public CustomerContainerReturn SearchCustomers(string searchText)
		{
			return _custRepo.SearchCustomerContainers(searchText);
		}

		/// <summary>
		/// Retrieve balance for customer
		/// </summary>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpGet]
		[ApiKeyedRoute("profile/customer/balance")]
		public CustomerBalanceOrderUpdatedModel CustomerBalance()
		{
			return _profileLogic.GetBalanceForCustomer(this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId);
		}

		/// <summary>
		/// Update user profile
		/// </summary>
		/// <param name="pwInfo">Password information</param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[ApiKeyedRoute("profile/password")]
		public OperationReturnModel<bool> UpdatePassword(UpdatePasswordModel pwInfo)
		{
			OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();
			try
			{
				returnValue.SuccessResponse = _profileLogic.UpdateUserPassword(this.AuthenticatedUser, pwInfo.Email, pwInfo.OriginalPassword, pwInfo.NewPassword);
			}
			catch (Exception ex)
			{
				returnValue.SuccessResponse = false;
				returnValue.ErrorMessage = ex.Message;
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
		public OperationReturnModel<UserProfileReturn> UpdateUser(UserProfileModel userInfo)
		{
			OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

			try
			{
				if (String.IsNullOrEmpty(userInfo.UserId)) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

				if (!_profileLogic.IsInternalAddress(userInfo.Email))
				{
					_profileLogic.UpdateUserProfile(this.AuthenticatedUser, userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName,
												  userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId,
												  true /* hard coded security for now */, userInfo.Customers, userInfo.Role);
				}

				retVal.SuccessResponse = _profileLogic.GetUserProfile(userInfo.Email);
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

		/// <summary>
		/// Create account
		/// </summary>
		/// <param name="account">Account</param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[ApiKeyedRoute("profile/account")]
		//[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
		public OperationReturnModel<AccountReturn> CreateAccount(Account account)
		{
			OperationReturnModel<AccountReturn> retVal = new OperationReturnModel<AccountReturn>();

			try
			{
				retVal.SuccessResponse = _profileLogic.CreateAccount(this.AuthenticatedUser, account.Name);
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

		/// <summary>
		/// Delete account (customer group)
		/// </summary>
		/// <param name="accountId">Id for account to delete</param>
		/// <returns></returns>
		[Authorize]
		[HttpDelete]
		[ApiKeyedRoute("profile/account/{accountId}")]
		public OperationReturnModel<bool> DeleteAccount(Guid accountId)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				_profileLogic.DeleteAccount(this.AuthenticatedUser, accountId);
				retVal.SuccessResponse = true;
			}
			catch (ApplicationException axe)
			{
				retVal.ErrorMessage = axe.Message;
				retVal.SuccessResponse = false;
				_log.WriteErrorLog("Application exception", axe);
			}
			catch (Exception ex)
			{
				retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
				retVal.SuccessResponse = false;
				_log.WriteErrorLog("Unhandled exception", ex);
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
		public OperationReturnModel<bool> UpdateAccount(Account account)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
			retVal.SuccessResponse = false;
			try
			{
				retVal.SuccessResponse = _profileLogic.UpdateAccount(this.AuthenticatedUser, account.Id, account.Name, account.Customers, account.AdminUsers);
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

		/// <summary>
		/// Retrieve account
		/// </summary>
		/// <param name="accountid">Account id</param>
		/// <returns></returns>
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

		/// <summary>
		/// Add user to account
		/// </summary>
		/// <param name="info">Account and user</param>
		/// <returns></returns>
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

		/// <summary>
		/// Retrieve all users for an account
		/// </summary>
		/// <param name="accountId">Account id</param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[ApiKeyedRoute("profile/account/{accountId}/users")]
		//[Authorization(new string[] { Core.Constants.ROLE_INTERNAL_DSM_FAM })] // TODO get proper roles
		public OperationReturnModel<AccountUsersReturn> GetAcountUsers(Guid accountId)
		{
			return new OperationReturnModel<AccountUsersReturn>() { SuccessResponse = _profileLogic.GetAccountUsers(accountId) };
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
		public OperationReturnModel<bool> RemoveUserFromAcocunt(Guid userId, Guid accountId)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				_profileLogic.RemoveUserFromAccount(this.AuthenticatedUser, accountId, userId);
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

		/// <summary>
		/// Add user to a customer
		/// </summary>
		/// <param name="info">Customer and user</param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[ApiKeyedRoute("profile/customer/user")]
		public OperationReturnModel<bool> AddUserToCustomer(CustomerAddUserModel info)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				_profileLogic.AddUserToCustomer(this.AuthenticatedUser, info.customerId, info.userId);
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

		/// <summary>
		/// Paged search of customers
		/// </summary>
		/// <param name="paging">Paging information</param>
		/// <param name="sort">Sort object</param>
		/// <param name="account">Account</param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[ApiKeyedRoute("profile/customer/")]
		public PagedResults<Customer> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort, [FromUri] string account = "", [FromUri] string terms = "")
		{
			if (paging.Sort == null && sort != null && !String.IsNullOrEmpty(sort.Order) && !String.IsNullOrEmpty(sort.Field))
			{
				paging.Sort = new List<SortInfo>() { sort };
			}
			return _profileLogic.CustomerSearch(this.AuthenticatedUser, terms, paging, account);
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

		/// <summary>
		/// Retrieve accounts
		/// </summary>
		/// <param name="accountFilter">Filter/search information</param>
		/// <returns></returns>
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

		/// <summary>
		/// Retrieve paged list of accounts
		/// </summary>
		/// <param name="paging">Paging information</param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[ApiKeyedRoute("profile/accounts")]
		public PagedResults<Account> Accounts(PagingModel paging)
		{
			return _profileLogic.GetPagedAccounts(paging);
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

		/// <summary>
		/// Retrieve all users for external user (Is this still used???)
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[ApiKeyedRoute("profile/user/{userid}/customers")]
		public OperationReturnModel<CustomerReturn> GetCustomersForExternalUser(Guid userid)
		{
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

		/// <summary>
		/// Upload user avatar
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[ApiKeyedRoute("profile/avatar")]
		public async Task<OperationReturnModel<bool>> UploadAvatar()
		{
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

				if (paramName.Equals("file"))
				{
					base64FileString = Convert.ToBase64String(buffer, Base64FormattingOptions.None);
				}

				if (paramName.Equals("name"))
				{
					using (var s = new System.IO.StreamReader(stream))
					{
						fileName = s.ReadToEnd();
					}
				}
			}

			try
			{
				returnValue.SuccessResponse = _avatarRepository.SaveAvatar(this.AuthenticatedUser.UserId, fileName, base64FileString);
			}
			catch (Exception e)
			{
				returnValue.SuccessResponse = false;
				returnValue.ErrorMessage = e.Message;
			}

			return returnValue;
		}

		/// <summary>
		/// Retrieve sales rep
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[ApiKeyedRoute("profile/salesrep")]
		public OperationReturnModel<bool> GetSalesRep()
		{
			// Get the DSR
			return new OperationReturnModel<bool>() { SuccessResponse = true };
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
		public OperationReturnModel<bool> GrantApplicationAccess(string email, string appname)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				AccessRequestType selectedApp = AccessRequestType.Undefined;

				switch (appname.ToLower())
				{
					case "kbit":
						selectedApp = AccessRequestType.KbitCustomer;
						break;
					case "powermenu":
						selectedApp = AccessRequestType.PowerMenu;
						break;
					default:
						break;
				}

				if (selectedApp == AccessRequestType.Undefined)
				{
					retVal.SuccessResponse = false;
					retVal.ErrorMessage = "Could not grant access to unknown application.";
				}
				else
				{
					_profileLogic.GrantRoleAccess(this.AuthenticatedUser, email, selectedApp);

					retVal.SuccessResponse = true;
				}
			}
			catch (Exception ex)
			{
				retVal.SuccessResponse = false;
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
		public OperationReturnModel<bool> RevokeApplicationAccess(string email, string appname)
		{
			OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

			try
			{
				AccessRequestType selectedApp = AccessRequestType.Undefined;

				switch (appname.ToLower())
				{
					case "kbit":
						selectedApp = AccessRequestType.KbitCustomer;
						break;
                    case "powermenu":
                        selectedApp = AccessRequestType.PowerMenu;
                        break;
					default:
						break;
				}

				if (selectedApp == AccessRequestType.Undefined)
				{
					retVal.SuccessResponse = false;
					retVal.ErrorMessage = "Could not revoke access from unknown application.";
				}
				else
				{
					_profileLogic.RevokeRoleAccess(this.AuthenticatedUser, email, selectedApp);

					retVal.SuccessResponse = true;
				}
			}
			catch (Exception ex)
			{
				retVal.SuccessResponse = false;
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
		public OperationReturnModel<bool> ForgotPassword(string emailAddress)
		{
			OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

			try
			{
				_passwordResetService.GeneratePasswordResetRequest(emailAddress);
				returnValue.SuccessResponse = true;
			}
			catch (Exception ex)
			{
				returnValue.SuccessResponse = false;
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
		public bool ValidateToken(ValidateTokenModel tokenModel)
		{
			return _passwordResetService.IsTokenValid(tokenModel.Token);
		}

		/// <summary>
		/// Change user password, using the forgot password token
		/// </summary>
		/// <param name="resetModel">Password change information</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("profile/forgotpassword/change")]
		public OperationReturnModel<bool> ChangeForgotPassword(ResetPasswordModel resetModel)
		{
			OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

			try
			{
				returnValue.SuccessResponse = _passwordResetService.ResetPassword(resetModel); ;
			}
			catch (Exception ex)
			{
				returnValue.SuccessResponse = false;
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
        public OperationReturnModel<DsrAlias> CreateDsrAlias(DsrAliasModel model) {
            OperationReturnModel<DsrAlias> retVal = new OperationReturnModel<DsrAlias>();

            try {
                retVal.SuccessResponse = _dsrAliasService.CreateDsrAlias(model.UserId, model.Email, new Dsr() { Branch = model.BranchId, DsrNumber = model.DsrNumber });
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not create alias";
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
                _dsrAliasService.DeleteDsrAlias(model.DsrAliasId, model.Email);

                retVal.SuccessResponse = true;
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not delete alias";
                _log.WriteErrorLog("Could not delete alias", ex);
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
        public OperationReturnModel<List<DsrAlias>> GetDsrAliases() {
            OperationReturnModel<List<DsrAlias>> retVal = new OperationReturnModel<List<DsrAlias>>();

            try {
                retVal.SuccessResponse = _dsrAliasService.GetAllDsrAliasesByUserId(AuthenticatedUser.UserId);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not get aliases for current user";
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
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
        public OperationReturnModel<List<DsrAlias>> GetDsrAliases(Guid userId) {
            OperationReturnModel<List<DsrAlias>> retVal = new OperationReturnModel<List<DsrAlias>>();

            try {
                retVal.SuccessResponse = _dsrAliasService.GetAllDsrAliasesByUserId(userId);
            } catch (Exception ex) {
                retVal.ErrorMessage = string.Format("Could not get aliases for speicified user {0}", userId);
                _log.WriteErrorLog(retVal.ErrorMessage, ex);
            }

            return retVal;
        }
        #endregion
	}
}