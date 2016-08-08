﻿using CommerceServer.Foundation;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Enumerations.SingleSignOn;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Extensions.SingleSignOn;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;

using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.PowerMenu;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.SingleSignOn;

using KeithLink.Svc.Impl.Helpers;

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class UserProfileLogicImpl : IUserProfileLogic {
        #region attributes
        private const string GUEST_USER_WELCOME = "GuestUserWelcome";
        private const string CREATED_USER_WELCOME = "CreatedUserWelcome";
        private const string RESET_PASSWORD = "ResetPassword";

		protected string CACHE_GROUPNAME { get { return "Profile"; } }
		protected string CACHE_NAME { get { return "Profile"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }


        private readonly ICacheRepository _cache;
        private readonly IUserProfileRepository _csProfile;
        private readonly ICustomerDomainRepository _extAd;
        private readonly IUserDomainRepository _intAd;
        private readonly IAccountRepository _accountRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IInternalUserAccessRepository _internalUserAccessRepo;
        private readonly IMessagingLogic _msgLogic;
        private readonly IMessageTemplateLogic _msgTemplateLogic;
		private readonly IEmailClient _emailClient;
		private readonly IEventLogRepository _eventLog;
		private readonly IOnlinePaymentsLogic _paymentLogic;
        private readonly IGenericQueueRepository _queue;
        private readonly IDsrAliasLogic _dsrAliasLogic;
		private readonly IPasswordResetLogic _passwordLogic;
        private readonly ISettingsLogic _settingsLogic;
        private readonly IOrderHistoryHeaderRepsitory _historyRepo;
        #endregion

        #region ctor
        public UserProfileLogicImpl(ICustomerDomainRepository externalAdRepo, IUserDomainRepository internalAdRepo, IUserProfileRepository commerceServerProfileRepo,
									ICacheRepository profileCache, IAccountRepository accountRepo, ICustomerRepository customerRepo,
                                    IOrderHistoryHeaderRepsitory orderHistoryRepository, IMessagingLogic messagingLogic, IEmailClient emailClient, 
                                    IEventLogRepository eventLog, IOnlinePaymentsLogic paymentLogic, IGenericQueueRepository queue, 
                                    IDsrAliasLogic dsrAliasLogic, IPasswordResetLogic passwordResetLogic, ISettingsLogic settingsLogic, 
                                    IMessageTemplateLogic messageTemplateLogic, IInternalUserAccessRepository internalUserAccessRepo) {
            _cache = profileCache;
            _extAd = externalAdRepo;
            _intAd = internalAdRepo;
            _csProfile = commerceServerProfileRepo;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _internalUserAccessRepo = internalUserAccessRepo;
            _msgLogic = messagingLogic;
            _msgTemplateLogic = messageTemplateLogic;
			_emailClient = emailClient;
			_eventLog = eventLog;
			_paymentLogic = paymentLogic;
            _queue = queue;
            _dsrAliasLogic = dsrAliasLogic;
			_passwordLogic = passwordResetLogic;
            _settingsLogic = settingsLogic;
            _historyRepo = orderHistoryRepository;
        }
        #endregion

        #region methods
		public string AddProfileImageUrl(Guid userId) {
            return String.Format("{0}{1}{2}", Configuration.MultiDocsProxyUrl, "avatar/", userId);
        }

		public void AddUserToCustomer(UserProfile addedBy, Guid customerId, Guid userId) {

            _customerRepo.AddUserToCustomer(addedBy.EmailAddress ,customerId, userId);

			GenerateUserAddedNotification(customerId, userId);
        }

        /// <summary>
        /// check that the customer name is longer the 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertCustomerNameLength(string customerName) {
            if (customerName.Length == 0)
                throw new ApplicationException("Customer name is blank");
        }

        /// <summary>
        /// make sure that there are not any special characters in the customer name
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertCustomerNameValidCharacters(string customerName) {
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ApplicationException("Invalid characters in customer name"); }
        }

        /// <summary>
        /// test to make sure that it is a correctly formatted email address
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddress(string emailAddress) {
            try {
                System.Net.Mail.MailAddress testAddress = new System.Net.Mail.MailAddress(emailAddress);
            } catch {
                throw new ApplicationException("Invalid email address");
            }
        }

        /// <summary>
        /// make sure that the email address is longer the 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddressLength(string emailAddress) {
            if (emailAddress.Length == 0)
                throw new ApplicationException("Email address is blank");
        }

        /// <summary>
        /// make sure that the email address does not already exist
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddressUnique(string emailAddress) {
            if (_extAd.GetUser(emailAddress) != null) {
                throw new ApplicationException("Email address is already in use");
            }
        }

        /// <summary>
        /// make sure that the first name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertFirstNameLength(string firstName) {
            if (firstName.Length == 0)
                throw new ApplicationException("First name is blank");
        }

        /// <summary>
        /// validate the values needed to create a guest profile
        /// </summary>
        /// <remarks>
        /// jwames - 9/15/2014 - original code
        /// </remarks>
        private void AssertGuestProfile(string emailAddress, string password) {
            AssertEmailAddress(emailAddress);
            AssertEmailAddressLength(emailAddress);
            AssertEmailAddressUnique(emailAddress);
            AssertPasswordComplexity(password);
            AssertPasswordLength(password);
        }

        /// <summary>
        /// make sure that the last name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertLastNameLength(string lastName) {
            if (lastName.Length == 0)
                throw new ApplicationException("Last name is blank");
        }

        /// <summary>
        /// make sure that the password meets our complexity rules of 1 upper case letter, 1 lower case letter, and 1 number
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertPasswordComplexity(string password) {
            if (PasswordMeetsComplexityRequirements(password) == false) {
                throw new ApplicationException("Password must contain 1 upper and 1 lower case letter and 1 number");
            }
        }

        /// <summary>
        /// make sure that the password is longer than 7 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertPasswordLength(string password) {
            if (password.Length < 7)
                throw new ApplicationException("Minimum password length is 7 characters");
        }

        /// <summary>
        /// make sure that the password does not contain any of the values stored in the attributes such as first or last name
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertPasswordVsAttributes(string password, string firstName, string lastName) {
            bool matched = false;

            //if (string.Compare(password, customerName, true) == 0) { matched = true; }
            if (string.Compare(password, firstName, true) == 0) { matched = true; }
            if (string.Compare(password, lastName, true) == 0) { matched = true; }

            if (matched) {
                throw new ApplicationException("Invalid password");
            }
        }

        /// <summary>
        /// make sure that the role name is a valid role
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertRoleName(string roleName) {
            bool found = false;

            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_ACCOUNTING, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_GUEST, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_OWNER, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_PURCHASINGAPPROVER, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_PURCHASINGBUYER, true) == 0) { found = true; }

            if (found == false) {
                throw new ApplicationException("Role name is unknown");
            }
        }

        /// <summary>
        /// make sure that the role name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertRoleNameLength(string roleName) {
            if (roleName.Length == 0) { throw new ApplicationException("Role name is blank"); }
        }

        /// <summary>
        /// validate all of the attributes of the user's profile
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName) {
            AssertCustomerNameLength(customerName);
            AssertCustomerNameValidCharacters(customerName);
            AssertEmailAddress(emailAddres);
            AssertEmailAddressLength(emailAddres);
            AssertFirstNameLength(firstName);
            AssertLastNameLength(lastName);
            AssertPasswordComplexity(password);
            AssertPasswordLength(password);
            //AssertPasswordVsAttributes(password, customerName, firstName, lastName);
            AssertPasswordVsAttributes(password, firstName, lastName);
            AssertRoleName(roleName);
            AssertRoleNameLength(roleName);
        }

        private List<ProfileMessagingPreferenceDetailModel> BuildPreferenceModelForEachNotificationType(List<UserMessagingPreferenceModel> currentMsgPrefs, string customerNumber) {
            var msgPrefModelList = new List<ProfileMessagingPreferenceDetailModel>();
            //loop through each notification type to load in model
            foreach (NotificationType notifType in Enum.GetValues(typeof(NotificationType))) {
                if (String.IsNullOrEmpty(EnumUtils<NotificationType>.GetDescription(notifType, string.Empty)))
                    continue; // don't include values for types without a description; those are for internal use

                var currentSelectedChannels = new List<ProfileChannelModel>();

                //find and add selected channels for current notification type
                var currentMsgPrefsByType = currentMsgPrefs.Where(a => (a.NotificationType.Equals(notifType) && a.CustomerNumber == customerNumber));
                foreach (var currentMsgPref in currentMsgPrefsByType) {
                    currentSelectedChannels.Add(new ProfileChannelModel() { Channel = currentMsgPref.Channel, Description = EnumUtils<Channel>.GetDescription(currentMsgPref.Channel, "") });
                }
                msgPrefModelList.Add(new ProfileMessagingPreferenceDetailModel() {
                    NotificationType = (NotificationType)notifType,
                    Description = EnumUtils<NotificationType>.GetDescription(notifType, ""),
                    SelectedChannels = currentSelectedChannels
                });

            }
            return msgPrefModelList;
        }
        
        private string CacheKey(string email) {
            return string.Format("{0}-{1}", "bek", email);
        }

        public AccountReturn CreateAccount(UserProfile createdBy, string name) {
            // call CS account repository -- hard code it for now
            Guid newAcctId = _accountRepo.CreateAccount(createdBy.EmailAddress, name);
            return new AccountReturn() { Accounts = new List<Account>() { new Account() { Name = name, Id = newAcctId } } };
        }

        public DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(email));

            return _dsrAliasLogic.CreateDsrAlias(userId, email, dsr);
        }

		public void DeleteAccount(UserProfile deletedBy, Guid accountId)
		{
			List<Customer> existingCustomers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());
			List<UserProfile> existingUsers = _csProfile.GetUsersForCustomerOrAccount(accountId);

			//Remove all existing customers and users
			foreach (Customer cust in existingCustomers)
				_accountRepo.RemoveCustomerFromAccount(deletedBy.EmailAddress, accountId, cust.CustomerId);
			foreach (UserProfile user in existingUsers)
				this.RemoveUserFromAccount(deletedBy, accountId, user.UserId);

			_accountRepo.DeleteAccount(deletedBy.EmailAddress, accountId);
		}

        public void DeleteDsrAlias(long dsrAliasId, string email) {
            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(email));

            _dsrAliasLogic.DeleteDsrAlias(dsrAliasId, email);
        }

        /// <summary>
        /// take the role name that is passed in and convert it to the name that is actually used in AD
        /// </summary>
        /// <returns>the name of the corresponding role from the config file</returns>
        /// <remarks>
        /// jwames - 4/2/2015 - original code
        /// </remarks>
        private string ConvertRoleName(string roleName) {
            if (roleName.Equals(Constants.ROLE_EXTERNAL_ACCOUNTING, StringComparison.InvariantCultureIgnoreCase))
                return Configuration.RoleNameAccounting;
            else if (roleName.Equals(Constants.ROLE_EXTERNAL_GUEST, StringComparison.InvariantCultureIgnoreCase))
                return Configuration.RoleNameGuest;
            else if (roleName.Equals(Constants.ROLE_EXTERNAL_OWNER, StringComparison.InvariantCultureIgnoreCase))
                return Configuration.RoleNameOwner;
            else if (roleName.Equals(Constants.ROLE_EXTERNAL_PURCHASINGAPPROVER, StringComparison.InvariantCultureIgnoreCase))
                return Configuration.RoleNameApprover;
            else if (roleName.Equals(Constants.ROLE_EXTERNAL_PURCHASINGBUYER, StringComparison.InvariantCultureIgnoreCase))
                return Configuration.RoleNameBuyer;
            else
                throw new ArgumentException("Unknown role name received");
        }

        /// <summary>
        /// create a Commerce Server User Profile for a BEK user
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <remarks>
        /// jwames - 8/29/2014 - original code
        /// </remarks>
        public void CreateBekUserProfile(string emailAddress) {
            System.DirectoryServices.AccountManagement.UserPrincipal bekUser = _intAd.GetUser(emailAddress);
            string fName = bekUser.DisplayName.Split(' ')[0];

            _csProfile.CreateUserProfile(emailAddress, emailAddress, fName, bekUser.Surname, bekUser.GetPhoneNumber(), GetBranchFromOU(bekUser.GetOrganizationalunit()));
        }

        /// <summary>
        /// create a guest account in AD and CS
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="password">their password</param>
        /// <param name="branchId">their selected branch</param>
        /// <returns>a completed user profile</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// jwames - 4/1/2015 - change AD structure
        /// </remarks>
        public UserProfileReturn CreateGuestUserAndProfile(UserProfile actingUser, string emailAddress, string password, string branchId) {
            if (emailAddress == null) throw new Exception( "email address cannot be null" );
            if (ProfileHelper.IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot create an account in External AD for an Internal User"); }
            if (password == null) throw new Exception( "password cannot be null" );

            AssertGuestProfile(emailAddress, password);

            _extAd.CreateUser(Configuration.RoleNameGuest,
                              emailAddress, 
                              password, 
                              Core.Constants.AD_GUEST_FIRSTNAME, 
                              Core.Constants.AD_GUEST_LASTNAME, 
                              Configuration.RoleNameGuest
                              );

            _csProfile.CreateUserProfile(actingUser.EmailAddress,
										 emailAddress,
                                         Core.Constants.AD_GUEST_FIRSTNAME,
                                         Core.Constants.AD_GUEST_LASTNAME,
                                         string.Empty,
                                         branchId
                                         );

                try {
                    var template = _msgTemplateLogic.ReadForKey( GUEST_USER_WELCOME );

                    if (template != null)
                        _emailClient.SendTemplateEmail( template, new List<string>() { emailAddress }, null, null, new { contactEmail = Configuration.BranchContactEmail( branchId ) } );
                } catch (Exception ex) {
                    //The registration probably shouldn't fail just because of an SMTP issue. So ignore this error and log
                    _eventLog.WriteErrorLog( "Error sending welcome email", ex );
                }

            return GetUserProfile(emailAddress);
        }

        /// <summary>
        /// create a user in AD and CS
        /// </summary>
        /// <param name="customerName">the company that the user works for</param>
        /// <param name="emailAddress">the user's email</param>
        /// <param name="password">their desired password</param>
        /// <param name="firstName">user's given name</param>
        /// <param name="lastName">user's surname</param>
        /// <param name="phone">telephone number</param>
        /// <param name="roleName">assigned role</param>
        /// <returns>a completed user profile</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
		public UserProfileReturn CreateUserAndProfile(UserProfile actingUser, string customerName, string emailAddress, 
                                                      string password, string firstName, string lastName, 
                                                      string phone, string roleName, string branchId) {
            if (ProfileHelper.IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot create an account in External AD for an Internal User"); }
            AssertUserProfile(customerName, emailAddress, password, firstName, lastName, phone, roleName);

            _extAd.CreateUser(customerName,
                                emailAddress,
                                password,
                                firstName,
                                lastName,
                                ConvertRoleName(roleName)
                                );

            try {
                _csProfile.CreateUserProfile(actingUser.EmailAddress,
                                             emailAddress,
                                             firstName,
                                             lastName,
                                             phone,
                                             branchId
                                             );

            } catch (Exception) {
                // if CS profile creation fails, delete the user from AD
                // too
                _extAd.DeleteUser(emailAddress);

                throw;
            }

            return GetUserProfile(emailAddress);
        }

        /// <summary>
        /// return all customers that a user has access to
        /// </summary>
        /// <param name="user">the current user</param>
        /// <param name="searchTerms">the text to search for</param>
        /// <param name="paging">paging information</param>
        /// <param name="account"></param>
        /// <returns>paged list of Customers</returns>
        /// <remarks>
        /// jwames - 4/9/2015 - add support for GOF power users
        /// jwames - 5/1/2015 - add DSR Alias support
        /// jwames - 5/10/2015 - cast to the DSR Alias Model
        /// </remarks>
        public Core.Models.Paging.PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, Core.Models.Paging.PagingModel paging, string account, CustomerSearchType searchType)
        {
            if (string.IsNullOrEmpty(searchTerms))
                searchTerms = "";

            if (!string.IsNullOrEmpty(account))
                return _customerRepo.GetPagedCustomersForAccount(paging, searchTerms, account.ToGuid().ToCommerceServerFormat(), searchType);

            if (ProfileHelper.IsInternalAddress(user.EmailAddress))
            {

                PagedResults<Customer> returnValue = new PagedResults<Customer>();

                if (user.IsDSR)
                {
                    if (!String.IsNullOrEmpty(user.DSRNumber) || user.DsrAliases != null)
                    {
                        // lookup customers by their assigned dsr number
                        //return _customerRepo.GetPagedCustomersForDSR(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, user.DSRNumber, user.BranchId, searchTerms);
                        returnValue = _customerRepo.GetPagedCustomersForDSR(paging, 
                                                                 searchTerms,
                                                                 (from DsrAliasModel d in user.DsrAliases
                                                                  select new Dsr() {
                                                                      Branch = d.BranchId,
                                                                      DsrNumber = d.DsrNumber
                                                                  }).ToList(),
                                                                  searchType);
                    }
                }
                if (user.IsDSM)
                {
                    if (!String.IsNullOrEmpty(user.DSMNumber))
                    {
                        // lookup customers by their assigned dsr number
                        returnValue = _customerRepo.GetPagedCustomersForDSM(paging, user.DSMNumber, user.BranchId, searchTerms, searchType);
                    }
                }
                else if (user.RoleName.Equals(Constants.ROLE_NAME_BRANCHIS) || ((user.RoleName.Equals(Constants.ROLE_NAME_POWERUSER) || user.RoleName.Equals(Constants.ROLE_NAME_MARKETING)) && user.BranchId != Constants.BRANCH_GOF))
                {
                    returnValue = _customerRepo.GetPagedCustomersForBranch(paging, user.BranchId, searchTerms, searchType);

                }
                else if (user.RoleName.Equals(Constants.ROLE_NAME_SYSADMIN) || ((user.RoleName.Equals(Constants.ROLE_NAME_POWERUSER) || user.RoleName.Equals(Constants.ROLE_NAME_MARKETING)) && user.BranchId == Constants.BRANCH_GOF))
                {
                    returnValue = _customerRepo.GetPagedCustomers(paging, searchTerms, searchType);
                }

                if (returnValue.Results != null)
                {
                    //For internal users, switch displayname to include branch id
                    Parallel.ForEach(returnValue.Results, customer =>
                    {
                        customer.DisplayName = string.Format("{0} ({1}) - {2}", customer.CustomerNumber, customer.CustomerBranch, customer.CustomerName);
                    });
                }
                

                return returnValue;

            } else { // external user
				if (user.RoleName == Constants.ROLE_NAME_KBITADMIN)
                    return _customerRepo.GetPagedCustomers(paging, searchTerms, searchType);
				else
                    return _customerRepo.GetPagedCustomersForUser(paging, user.UserId, searchTerms, searchType);
            }
        }
        
        /// <summary>
        /// take all of the fields from the commerce server profile and put them into our custom object and load other custom data
        /// </summary>
        /// <param name="csProfile">profile data from commerce server</param>
        /// <returns>a completed user profile object</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - derived from CombineCSAndADProfile method
        /// jwames - 4/13/2015 - handle the DSM number when it is two or three digits
        /// jwames - 5/1/2015 - add dsr alias support
        /// </remarks>
        public UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false) {
            //List<Customer> userCustomers;
            string dsrRole = string.Empty;
            string dsrNumber = string.Empty;
            string dsmNumber = string.Empty;
            string dsmRole = string.Empty;
            string userRole = string.Empty;
            string userBranch = string.Empty;
            bool isInternalUser = ProfileHelper.IsInternalAddress(csProfile.Email);
            UserPrincipal adUser = null;
            bool isKbitCustomer = false;
            bool isPowerMenuCustomer = false;
            bool isPowerMenuAdmin = false;
            UserProfile retVal = new UserProfile();

            try {
                if (isInternalUser) {
                    adUser = _intAd.GetUser(csProfile.Email);
                    // if the user is not found in AD return null
                    if (adUser == null) { return null; }

                    DirectoryEntry directoryEntry = (DirectoryEntry)adUser.GetUnderlyingObject();
                    List<string> internalUserRoles = _intAd.GetAllGroupsUserBelongsTo(adUser, Configuration.InternalUserRoles);

                    userBranch = GetBranchFromOU(adUser.GetOrganizationalunit());

                    if (internalUserRoles.Intersect(Configuration.BekSysAdminRoles).Count() > 0) {
                        userRole = Constants.ROLE_NAME_SYSADMIN;
                        isPowerMenuAdmin = true;
                    } else if (internalUserRoles.Intersect(Constants.MIS_ROLES).Count() > 0) {
                        userRole = Constants.ROLE_NAME_BRANCHIS;
                        isPowerMenuAdmin = true;
                        //userBranch = internalUserRoles.Intersect(Constants.MIS_ROLES).FirstOrDefault().ToString().Substring(0, 3);
                    } else if (internalUserRoles.Intersect(Constants.POWERUSER_ROLES).Count() > 0) {
                        userRole = Constants.ROLE_NAME_POWERUSER;
                        //userBranch = internalUserRoles.Intersect(Constants.POWERUSER_ROLES).FirstOrDefault().ToString().Substring(0, 3);
                    } else if (internalUserRoles.Intersect(Constants.MARKETING_ROLES).Count() > 0) {
                        userRole = Constants.ROLE_NAME_MARKETING;
                        //userBranch = internalUserRoles.Intersect(Constants.POWERUSER_ROLES).FirstOrDefault().ToString().Substring(0, 3);
                    } else if (internalUserRoles.Intersect(Constants.DSM_ROLES).Count() > 0) {
                        dsmRole = internalUserRoles.Intersect(Constants.DSM_ROLES).FirstOrDefault().ToString();
                        userRole = Constants.ROLE_NAME_DSM;
                        //userBranch = dsmRole.Substring(0, 3);
                        //dsmNumber = StringExtensions.ToInt(adUser.Description) != null ? adUser.Description : string.Empty;
                        if (adUser.Description.Length == 3) {
                            dsmNumber = adUser.Description.Substring(1, 2);
                        } else if (adUser.Description.Length == 2) {
                            dsmNumber = adUser.Description;
                        }
                    } else if (internalUserRoles.Intersect(Constants.DSR_ROLES).Count() > 0) {
                        dsrRole = internalUserRoles.Intersect(Constants.DSR_ROLES).FirstOrDefault().ToString();
                        userRole = Constants.ROLE_NAME_DSR;
                        dsrNumber = StringExtensions.ToInt(adUser.Description) != null ? adUser.Description : string.Empty;
                        //userBranch = dsrRole.Substring(0, 3);
                    } else {
                        userRole = Constants.ROLE_NAME_GUEST;
                    }
                } else {
                    adUser = _extAd.GetUser(csProfile.Email);

                    if (_extAd.HasAccess(csProfile.Email, Configuration.AccessGroupKbitAdmin)) {
                        userRole = Constants.ROLE_NAME_KBITADMIN;
                        isKbitCustomer = true;
                    } else {
                        userRole = GetUserRole(csProfile.Email);
                        isKbitCustomer = _extAd.HasAccess(csProfile.Email, Configuration.AccessGroupKbitCustomer);
                    }

                    userBranch = csProfile.DefaultBranch;
                    isKbitCustomer = _extAd.HasAccess(csProfile.Email, Configuration.AccessGroupKbitCustomer);
                    isPowerMenuCustomer = _extAd.HasAccess(csProfile.Email, Configuration.AccessGroupPowerMenuCustomer);
                }

                string userNameToken = string.Concat(adUser.SamAccountName, "-", DateTime.Now.ToString("yyyyMMddHHmmss"));
                byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(userNameToken);
                string tokenBase64 = Convert.ToBase64String(tokenBytes);


                retVal.UserId = Guid.Parse(csProfile.Id);
                retVal.IsInternalUser = ProfileHelper.IsInternalAddress(csProfile.Email);
                retVal.PasswordExpired = (isInternalUser) ? false : _extAd.IsPasswordExpired(csProfile.Email);
                retVal.LastActivity = csProfile.LastActivityDate;
                retVal.LastLogin = csProfile.LastLoginDate;
                retVal.FirstName = csProfile.FirstName;
                retVal.LastName = csProfile.LastName;
                retVal.EmailAddress = csProfile.Email;
                retVal.PhoneNumber = csProfile.Telephone;
                retVal.CustomerNumber = csProfile.DefaultCustomer;
                retVal.BranchId = userBranch;
                retVal.RoleName = userRole;
                retVal.DSMRole = dsmRole;
                retVal.DSRNumber = dsrNumber;
                retVal.DSMNumber = dsmNumber;
                //retVal.UserCustomers = userCustomers;
                retVal.ImageUrl = AddProfileImageUrl(Guid.Parse(csProfile.Id));
                retVal.UserName = adUser.SamAccountName;
                retVal.UserNameToken = tokenBase64;
                retVal.IsKBITCustomer = isKbitCustomer;
                retVal.IsPowerMenuCustomer = isPowerMenuCustomer;
                retVal.PowerMenuPermissionsUrl = (isPowerMenuCustomer) ? String.Format(Configuration.PowerMenuPermissionsUrl, csProfile.Email) : "";
                retVal.PowerMenuLoginUrl = (isInternalUser) ? "" : GetPowerMenuLoginUrl(Guid.Parse(csProfile.Id), csProfile.Email, adUser.SamAccountName);
                retVal.PowerMenuGroupSetupUrl = (isPowerMenuAdmin) ? String.Format(Configuration.PowerMenuGroupSetupUrl) : "";
#if DEMO
		    retVal.IsDemo = true;
#endif
                if (isInternalUser) {
                    try {
                        retVal.DsrAliases = _dsrAliasLogic.GetAllDsrAliasesByUserId(retVal.UserId);
                        if (retVal.DSRNumber.Length > 0) { retVal.DsrAliases.Add(new DsrAliasModel() { BranchId = retVal.BranchId, DsrNumber = retVal.DSRNumber }); }
                    } catch {

                    }
                }

                return retVal;
            } catch (Exception ex) {

                _eventLog.WriteErrorLog(string.Format("Error retrieving user {0} information", csProfile.Email), ex);
                return null;

            }
        }

        private string GenerateTemporaryPassword() {
            string generatedPassword = NewPassword();

            for (int i = 0; !PasswordMeetsComplexityRequirements(generatedPassword) && i < 8; i++)
                generatedPassword = NewPassword();

            return generatedPassword;
        }

        private void GenerateUserAddedNotification(Guid customerId, Guid userId) {
            try {
                //Lookup customer
                var customer = _customerRepo.GetCustomerById(customerId);
                //Lookup User
                var user = _csProfile.GetCSProfile(userId);

                var notifcation = new HasNewsNotification() {
                    CustomerNumber = customer.CustomerNumber,
                    BranchId = customer.CustomerBranch,
                    Subject = string.Format("New user added to {0}-{1}", customer.CustomerNumber, customer.CustomerName),
                    Notification = string.Format("User {0} has been added to {1}-{2}", user.Email, customer.CustomerNumber, customer.CustomerName),
                    DSRDSMOnly = true
                };
                _queue.PublishToQueue(notifcation.ToJson(), Configuration.RabbitMQNotificationServer,
                            Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
                            Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
            } catch (Exception ex) {
                _eventLog.WriteErrorLog("Error generating DSR/DSM new user notification", ex);
            }
        }

        public AccountReturn GetAccounts(AccountFilterModel accountFilters) {
            List<Account> retAccounts = new List<Account>();

            if (accountFilters != null) {
                if (accountFilters.UserId.HasValue) {

					var account = _accountRepo.GetAccountsForUser(accountFilters.UserId.Value);
					if (account != null && account.Count > 0)
					{
						retAccounts.AddRange(_accountRepo.GetAccountsForUser(accountFilters.UserId.Value));
					}
					else
					{
						//See if they are assigned to a customer that is a member of a customer group
						var accounts = _accountRepo.GetAccounts();
						var customers = _customerRepo.GetCustomersForUser(accountFilters.UserId.Value);
						var userAccounts = FindAccountsForCustomers(accounts, customers);

						retAccounts.AddRange(userAccounts);
					}
                }
                if (!String.IsNullOrEmpty(accountFilters.Wildcard)) {
                    retAccounts.AddRange(_accountRepo.GetAccounts().Where(x => x.Name.ToLower().Contains(accountFilters.Wildcard.ToLower())));
                }
            } else
                retAccounts = _accountRepo.GetAccounts();

            foreach (var acct in retAccounts) {
                acct.Customers = _customerRepo.GetCustomersForAccount(acct.Id.ToCommerceServerFormat());
            }
            // TODO: add logic to filter down for internal administration versus external owner

            return new AccountReturn() { Accounts = retAccounts.Distinct(new AccountComparer()).ToList() };
        }

		public Account GetAccount(UserProfile user, Guid accountId)
		{
            Account acct = _accountRepo.GetAccounts().Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());

			foreach (var cust in acct.Customers)
			{
				cust.CanMessage = true;
				if (user.IsDSR && user.DSRNumber != cust.DsmNumber)
					cust.CanMessage = false;
				else if (user.IsDSM && user.DSMNumber != cust.DsmNumber)
					cust.CanMessage = false;
			}


            acct.AdminUsers = _csProfile.GetUsersForCustomerOrAccount(accountId);
            acct.CustomerUsers = new List<UserProfile>();
            foreach (Customer c in acct.Customers) {
				var users = _csProfile.GetUsersForCustomerOrAccount(c.CustomerId);
				foreach (var custUser in users)
				{
					custUser.CanMessage = true;
					if (user.IsDSR && user.DSRNumber != c.DsmNumber)
						custUser.CanMessage = false;
					else if (user.IsDSM && user.DSMNumber != c.DsmNumber)
						custUser.CanMessage = false;
				}
                acct.CustomerUsers.AddRange(users);
            }
            acct.CustomerUsers = acct.CustomerUsers
				.OrderByDescending(o => o.CanMessage)
                                    .GroupBy(x => x.UserId)
                                    .Select(grp => grp.First())
									.ToList();

			foreach (var up in acct.AdminUsers)
				up.RoleName = GetUserRole(up.EmailAddress);

            foreach (var up in acct.CustomerUsers)
                up.RoleName = GetUserRole(up.EmailAddress);

			


            return acct;
        }

        public AccountUsersReturn GetAccountUsers(Guid accountId) {
            List<Account> allAccounts = _accountRepo.GetAccounts();
            Account acct = allAccounts.Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());

            AccountUsersReturn usersReturn = new AccountUsersReturn();
            usersReturn.AccountUserProfiles = _csProfile.GetUsersForCustomerOrAccount(accountId);
            usersReturn.CustomerUserProfiles = new List<UserProfile>();
            foreach (Customer c in acct.Customers) {
                usersReturn.CustomerUserProfiles.AddRange(_csProfile.GetUsersForCustomerOrAccount(c.CustomerId));
            }

            usersReturn.CustomerUserProfiles = usersReturn.CustomerUserProfiles
                .GroupBy(u => u.UserId)
                .Select(grp => grp.First())
                .ToList();


            foreach (var up in usersReturn.CustomerUserProfiles)
                up.RoleName = GetUserRole(up.EmailAddress);


            return usersReturn;
        }

        public CustomerBalanceOrderUpdatedModel GetBalanceForCustomer(string customerId, string branchId) {
            var returnModel = new CustomerBalanceOrderUpdatedModel();

            returnModel.LastOrderUpdate = _historyRepo.ReadLatestOrderDate(new UserSelectedContext() { BranchId = branchId, CustomerId = customerId });
            returnModel.balance = _paymentLogic.GetCustomerAccountBalance(customerId, branchId);

            return returnModel;
        }

        /// <summary>
        /// translate the AD branch name to mainframe branch name
        /// </summary>
        /// <param name="OU">user's organization unit</param>
        /// <returns>3-digit branch name</returns>
        /// <remarks>
        /// jwames - 10/9/2014 - original code
        /// jwames - 4/9/2015 - add GOF and use constants
        /// </remarks>
        private string GetBranchFromOU(string OU) {
            switch (OU) {
                case "FABQ":
                    return Constants.BRANCH_FAQ;
                case "FAMA":
                case "FDFW":
                case "FHST":
                case "FLRK":
                case "FOKC":
                case "FSAN":
                    return OU.Substring(0, 3);
                case "FFGO":
                    return Constants.BRANCH_GOF;
                default:
                    return null;
            }
        }

        public Customer GetCustomerByCustomerNumber(string customerNumber, string branchId) {
            Customer customer = _customerRepo.GetCustomerByCustomerNumber(customerNumber, branchId);

            // add customerusers to customer properties
            customer.CustomerUsers = new List<UserProfile>();
            List<UserProfile> users = _csProfile.GetUsersForCustomerOrAccount(customer.CustomerId);
            Dictionary<string, UserProfile> dic = new Dictionary<string, UserProfile>();
            foreach(UserProfile user in users)
            {
                if (dic.Keys.Contains(user.EmailAddress) == false)
                    dic.Add(user.EmailAddress, user);
            }
            customer.CustomerUsers.AddRange(dic.Values);

            return customer;
        }

        public List<Customer> GetCustomersForExternalUser(Guid userId) {
            Core.Models.Generated.UserProfile profile = _csProfile.GetCSProfile(userId);

            if (ProfileHelper.IsInternalAddress(profile.Email)) {
                throw new ApplicationException("This call is not supported for internal users.");
            } else {
                return _customerRepo.GetCustomersForUser(userId);
            }
        }

        public Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId) {
            return _customerRepo.GetCustomerForUser(customerNumber, branchId, userId);
        }

        public List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId) {
            return _dsrAliasLogic.GetAllDsrAliasesByUserId(userId);
        }

        public List<UserProfile> GetInternalUsersWithAccessToCustomer(string customerNumber, string branchId) {
            List<UserProfile> usersWithAccess = new List<UserProfile>();

            try {
                List<InternalUserAccess> allUsersForCustomer = _internalUserAccessRepo.GetAllUsersWithAccessToCustomer( new UserSelectedContext() { BranchId = branchId, CustomerId = customerNumber } );

                foreach (InternalUserAccess user in allUsersForCustomer) {
                    UserProfileReturn upToAddToList = GetUserProfile( user.EmailAddress );

                    if (upToAddToList.UserProfiles != null && upToAddToList.UserProfiles.Count > 0) {
                        usersWithAccess.Add( upToAddToList.UserProfiles[0] );
                    }
                }
            } catch (Exception ex) {
                _eventLog.WriteErrorLog( string.Format( "Error retrieving internal users for {0} - {1}", customerNumber, branchId ) );
            }

            return usersWithAccess;
        }

        public List<ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid) {
            var currentMessagingPreferences = _msgLogic.ReadMessagingPreferences(guid);
            var userCustomers = _customerRepo.GetCustomersForUser(guid);

            var returnedMsgPrefModel = new List<ProfileMessagingPreferenceModel>();
            //first load user preferences
            returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel() {
                Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, null)
            });
            //then load customer preferences
            foreach (var currentCustomer in userCustomers) {
                returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel() {
                    CustomerNumber = currentCustomer.CustomerNumber,
                    BranchId = currentCustomer.CustomerBranch,
                    Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, currentCustomer.CustomerNumber)
                });
            }

            return returnedMsgPrefModel;
        }

		public List<ProfileMessagingPreferenceModel> GetMessagingPreferencesForCustomer(Guid guid, string customerId, string branchId)
		{
			var currentMessagingPreferences = _msgLogic.ReadMessagingPreferences(guid);
			var customer = _customerRepo.GetCustomerByCustomerNumber(customerId, branchId);
			if (customer == null)
				return null;


			var returnedMsgPrefModel = new List<ProfileMessagingPreferenceModel>();
			//first load user preferences
			returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel()
			{
				Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, null)
			});
			//then load customer preferences
			
			returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel()
			{
				CustomerNumber = customer.CustomerNumber,
				BranchId = customer.CustomerBranch,
				Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, customer.CustomerNumber)
			});

			return returnedMsgPrefModel;
		}

        private string GetPowerMenuLoginUrl(Guid userId, string userName, string samAccountName) {
            string returnValue = String.Empty;

            List<Customer> customers = GetCustomersForExternalUser(userId);

            string customerNumbers = string.Join("|", (from customer in customers
                                                       where customer.IsPowerMenu == true
                                                       select customer.CustomerNumber));

            if (customerNumbers.Length > 0) {
                string password = String.Concat(samAccountName, userId.ToString().Substring(0, 4));
                returnValue = String.Format(Configuration.PowerMenuLoginUrl, userName, password, customerNumbers);
            }

            return returnValue;
        }

        /// <summary>
        /// return all customers for user
        /// </summary>
        /// <param name="user">the current user</param>
        /// <param name="search">search text</param>
        /// <returns>list of customers that a user has access to</returns>
        /// <remarks>
        /// jwames - 4/9/2015 - add support for GOF PowerUser
        /// jwames - 5/1/2015 - add dsr alias support
        /// </remarks>
        public List<Customer> GetNonPagedCustomersForUser(UserProfile user, string search = "") {
            List<Customer> allCustomers = new List<Customer>();
            if (string.IsNullOrEmpty(search))
                search = "";
            if (ProfileHelper.IsInternalAddress(user.EmailAddress)) {
                if (!String.IsNullOrEmpty(user.DSRNumber)) {
                    // lookup customers by their assigned dsr number
                    //allCustomers = _customerRepo.GetCustomersForDSR(user.DSRNumber, user.BranchId);
                    allCustomers = _customerRepo.GetCustomersForDSR((from DsrAlias d in user.DsrAliases
                                                                     select new Dsr() { Branch = d.BranchId, 
                                                                                        DsrNumber = d.DsrNumber })
                                                                     .ToList());
                } else if (!String.IsNullOrEmpty(user.DSMRole)) {
                    //lookup customers by their assigned dsm number
                    _customerRepo.GetCustomersForDSM(user.DSMNumber, user.BranchId);
                } else if (user.RoleName.Equals(Constants.ROLE_NAME_BRANCHIS) || (user.RoleName.Equals(Constants.ROLE_NAME_POWERUSER) && user.BranchId != Constants.BRANCH_GOF)) {
                    if (search.Length >= 3)
                        allCustomers = _customerRepo.GetCustomersByNameSearchAndBranch(search, user.BranchId);
                    else
                        allCustomers = _customerRepo.GetCustomersForUser(user.UserId);
                } else if (user.RoleName.Equals(Constants.ROLE_NAME_SYSADMIN) || user.RoleName.Equals(Constants.ROLE_NAME_MARKETING) || (user.RoleName.Equals(Constants.ROLE_NAME_POWERUSER) && user.BranchId.Equals(Constants.BRANCH_GOF))){ // assume admin user with access to all customers or a PowerUser from GOF
                    if (search.Length >= 3)
                        allCustomers = _customerRepo.GetCustomersByNameSearch(search);
                    else
                        allCustomers = _customerRepo.GetCustomers(); //use the use the user organization object for customer filtering
                }
            } else // external user
			{
                allCustomers = _customerRepo.GetCustomersForUser(user.UserId);
            }



            return allCustomers;
        }

		/// <summary>
		/// Returns a paged list of Accounts
		///		Additional searching options can be set to filter the accounts by user or customer
		///		To search for accounts by Customer
		///			paging.Type = "customer"
		///			paging.Terms = either customer number or name (partial matches work)
		///		To search for accounts by User
		///			paging.Type = "user"
		///			paing.Terms = user email address. Partial matches do not work, must be the full email address
		/// </summary>
		/// <param name="paging"></param>
		/// <returns>Paged list of accounts</returns>
        public PagedResults<Account> GetPagedAccounts(PagingModel paging) {
            var accounts = _accountRepo.GetAccounts();

            if (!string.IsNullOrEmpty(paging.Type) && paging.Type.Equals("User", StringComparison.CurrentCultureIgnoreCase))
	
			{
	
				var user = this.GetUserProfile(paging.Terms.Trim());
	
				if (user != null)
				{
	
					//This will find if they are an admin of a customer group
					var account = _accountRepo.GetAccountsForUser(user.UserProfiles[0].UserId);
					if (account != null && account.Count >0)
					{
						return new PagedResults<Account>() { Results = account, TotalResults = account.Count };
					}
	
					//See if they are assigned to a customer that is a member of a customer group
					var customers = _customerRepo.GetCustomersForUser(user.UserProfiles[0].UserId);
					var userAccounts = FindAccountsForCustomers(accounts, customers);
	
					return new PagedResults<Account>() { TotalResults = userAccounts.Count, Results = userAccounts };
				}
				return new PagedResults<Account>() { TotalResults = 0, Results = new List<Account>() };
			}
	
			else if (!string.IsNullOrEmpty(paging.Type) && paging.Type.Equals("Customer", StringComparison.CurrentCultureIgnoreCase))
			{
	
				var customers = _customerRepo.GetCustomersByNameOrNumber(paging.Terms.Trim());
				var userAccounts = FindAccountsForCustomers(accounts, customers);
	
				return new PagedResults<Account>() { TotalResults = userAccounts.Count, Results = userAccounts };
			}
	
			else
			{
				return accounts.AsQueryable().GetPage(paging);
			}
        }

		private static List<Account> FindAccountsForCustomers(List<Account> accounts, List<Customer> customers)
		{

			var userAccounts = new List<Account>();

			foreach (var cust in customers)
			{

				if (cust.AccountId.HasValue)
				{

					var acct = accounts.Where(a => a.Id == cust.AccountId).FirstOrDefault();

					if (acct != null)

						userAccounts.Add(acct);

				}

			}

			return userAccounts;
		}

        /// <summary>
        /// get a user profile from commerce server
        /// </summary>
        /// <param name="userId">commerce server's user id</param>
        /// <returns>user profile object</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public UserProfileReturn GetUserProfile(Guid userId, bool includeLastOrderDate = true) {
            // search commerce server 
            Core.Models.Generated.UserProfile csUserProfile = _csProfile.GetCSProfile(userId);

            UserProfileReturn retVal = new UserProfileReturn();

            if (csUserProfile == null) {
            } else {
                retVal.UserProfiles.Add(FillUserProfile(csUserProfile, includeLastOrderDate));
            }

            return retVal;
        }

        /// <summary>
        /// get the user profile
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// jwames - 8/29/2014 - create a profile for a BEK user if it does not exist
        /// </remarks>
        public UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false) {
            UserProfileReturn retVal = new UserProfileReturn();

            // check for cached user profile first
            Core.Models.Profile.UserProfile profile = _cache.GetItem<UserProfile>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(emailAddress));

            if (ProfileHelper.IsInternalAddress(emailAddress).Equals(false) && profile != null) {
                profile.PasswordExpired = _extAd.IsPasswordExpired(emailAddress); // always check password expired status; even when cached...
            }

            if (profile != null) {
                retVal.UserProfiles.Add(profile);
                return retVal;
            }

            // search commerce server next
            Core.Models.Generated.UserProfile csUserProfile = _csProfile.GetCSProfile(emailAddress);

            if (csUserProfile == null) {
                if (ProfileHelper.IsInternalAddress(emailAddress)) {
                    CreateBekUserProfile(emailAddress);

                    return GetUserProfile(emailAddress);
                }
            } else {
                retVal.UserProfiles.Add(FillUserProfile(csUserProfile, includeTermInformation: true));
            }

            // add to cache if found
            if (retVal.UserProfiles.Count > 0) {
                UserProfile currentUser = retVal.UserProfiles.FirstOrDefault();

                if(currentUser != null && !string.IsNullOrEmpty(currentUser.EmailAddress)) {
                    _cache.AddItem<UserProfile>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, 
                                                CacheKey(currentUser.EmailAddress), TimeSpan.FromHours(2), currentUser);
                }
            }

            return retVal;
        }

        /// <summary>
        /// get the user profile by guid
        /// </summary>
        /// <remarks>
        /// jmmcmillan - 10/6/2014 - documented
        /// </remarks>
        public UserProfileReturn GetUserProfileByGuid(Guid UserId) {

			//TODO: This is a Foundation Service Query. It should be moved to the profile repository
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Id"] = UserId.ToCommerceServerFormat();
            profileQuery.SearchCriteria.Model.DateModified = DateTime.Now;

            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("PhoneNumber");

            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            UserProfileReturn retVal = new UserProfileReturn();

            if (profileResponse.Count == 0) {
                /*
                 Throw profile not found exception??
                 */
            } else {
                retVal.UserProfiles.Add(FillUserProfile((Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0]));
            }

            return retVal;
        }

        //private string GetUserRole(UserPrincipal user) {
        //    PrincipalSearchResult<Principal> groups = user.GetGroups();

        //    foreach (GroupPrincipal group in groups) {
        //        group.Name
        //    }

        //    return null;
        //}

        /// <summary>
        /// get the role assigned to the specified user
        /// </summary>
        /// <param name="email">the user's email address</param>
        /// <returns>user's role name</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        private string GetUserRole(string email) {
            string roleName = null;

            if (ProfileHelper.IsInternalAddress(email)) {
                //roleName = _intAd.
                roleName = "owner";
            } else {
                roleName = _extAd.GetUserGroup(email, new List<string>() { "owner", "approver", "buyer", "accounting", "guest" });
            }

            return roleName;
        }

        public UserProfileReturn GetUsers(UserFilterModel userFilters) {
            if (userFilters != null) {
                if (userFilters.AccountId.HasValue)
                {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.AccountId.Value) };
                }
                else if (userFilters.CustomerId.HasValue)
                {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.CustomerId.Value) };
                }
                else if (userFilters.Type != null && userFilters.Type.Equals(Constants.EMAILMASK_BRANCHSYSTEMALERT, StringComparison.CurrentCultureIgnoreCase))
                {
                    List<Customer> customers = _customerRepo.GetCustomersForBranch(userFilters.Branch);
                    List<Core.Models.Profile.UserProfile> profiles = new List<Core.Models.Profile.UserProfile>();
                    foreach (Customer customer in customers)
                    {
                        if (customer.AccountId != null)
                        {
                            List<UserProfile> customerusers = _csProfile.GetUsersForCustomerOrAccount(customer.CustomerId);
                            profiles.AddRange(_csProfile.GetUsersForCustomerOrAccount(customer.CustomerId));
                        }
                    }
                    // extra measure to make sure we aren't getting duplicates
                    List<Core.Models.Profile.UserProfile> dprofiles = new List<Core.Models.Profile.UserProfile>();
                    dprofiles.AddRange(profiles.Distinct());
                    return new UserProfileReturn() { UserProfiles = dprofiles };
                }
                else if (userFilters.Type != null && userFilters.Type.Equals(Constants.EMAILMASK_ALLSYSTEMALERT, StringComparison.CurrentCultureIgnoreCase))
                {
                    // special case for systemwide alerts
                    List<Core.Models.Profile.UserProfile> profiles = new List<Core.Models.Profile.UserProfile>();
                    profiles.AddRange(_csProfile.GetInternalUsers());
                    profiles.AddRange(_csProfile.GetExternalUsers());
                    // extra measure to make sure we aren't getting duplicates
                    List<Core.Models.Profile.UserProfile> dprofiles = new List<Core.Models.Profile.UserProfile>();
                    dprofiles.AddRange(profiles.Distinct());
                    return new UserProfileReturn() { UserProfiles = dprofiles };
                }
                else if (!String.IsNullOrEmpty(userFilters.Email))
                {
                    return GetUserProfile(userFilters.Email);
                }
            }

            throw new ApplicationException("No filter provided for users");
        }

        /// <summary>
        /// grant access in the customer AD for the user to the specified role and refresh the user cache
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="roleName">the requested role</param>
        /// <remarks>
        /// jwames - 3/10/2015 - original code
        /// </remarks>
        public void GrantRoleAccess(UserProfile updatedBy, string emailAddress, AccessRequestType requestedApp) {
            string appRoleName;

            switch (requestedApp) {
                case AccessRequestType.KbitCustomer:
                    appRoleName = Configuration.AccessGroupKbitCustomer;
                    break;
                case AccessRequestType.PowerMenu:
                    appRoleName = Configuration.AccessGroupPowerMenuCustomer;
                    break;
                default:
                    return;
            }

            _eventLog.WriteInformationLog(string.Format("Granting role access in active directory ({0}, {1})", emailAddress, appRoleName));
			
            _extAd.GrantAccess(updatedBy.EmailAddress, emailAddress, appRoleName);

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(emailAddress));

            switch (requestedApp) {
                case AccessRequestType.KbitCustomer:
                    RequestKbitAccess(emailAddress);
                    break;
                case AccessRequestType.PowerMenu:
                    SetupPowerMenuForAccount( emailAddress );
                    break;
                default:
                    break;
            }
        }

        private string NewPassword() {
            Random rnd = new Random();
            string generatedPassword = System.Web.Security.Membership.GeneratePassword(8, 1);
            return Regex.Replace(generatedPassword, @"[^a-zA-Z0-9]", r => rnd.Next(0, 9).ToString());
        }

        private bool PasswordMeetsComplexityRequirements(string password) {
            return System.Text.RegularExpressions.Regex.IsMatch(password, Core.Constants.REGEX_PASSWORD_PATTERN);
        }

        private void RemoveKbitAccess(string emailAddress) {
            // create a new kbit app request
            KbitCustomerAccessRequest kbitAppRequest = new KbitCustomerAccessRequest();

            // get the user's profile
            UserProfileReturn userInfo = GetUserProfile(emailAddress, false);
            kbitAppRequest.UserName = userInfo.UserProfiles[0].UserName;

            // do not load customers because we are removing all of them

            SubmitExternalApplicationRequest(kbitAppRequest);
        }

        private void RemovePowerMenuAccess(string emailAddress) {

            // Create the powermenu specific request that gets serialized to xml

            // get the users profile
            UserProfileReturn userInfo = GetUserProfile( emailAddress, false );

            // get all customers for the user
            List<Customer> customers = GetCustomersForExternalUser( userInfo.UserProfiles[0].UserId );

            // create a request for every powermenu customer
            PowerMenuSystemRequestModel powerMenuRequest = BuildPowerMenuRequest( customers, userInfo, PowerMenuSystemRequestModel.Operations.Delete ); 

            SendPowerMenuRequests( powerMenuRequest, emailAddress );
        }

        private PowerMenuSystemRequestModel BuildPowerMenuRequest( List<Customer> customers, UserProfileReturn userInfo, PowerMenuSystemRequestModel.Operations operation ) {
            PowerMenuSystemRequestModel powerMenuRequest = new PowerMenuSystemRequestModel();

            powerMenuRequest = (from customer in customers
                                 where customer.IsPowerMenu == true
                                 select new PowerMenuSystemRequestModel() {
                                     User = new PowerMenuSystemRequestUserModel() {
                                         Username = userInfo.UserProfiles[0].EmailAddress,
                                         Password = String.Concat(userInfo.UserProfiles[0].UserName, userInfo.UserProfiles[0].UserId.ToString().Substring(0,4)),
                                         ContactName = userInfo.UserProfiles[0].EmailAddress,
                                         CustomerNumber = customer.CustomerNumber,
                                         EmailAddress = userInfo.UserProfiles[0].EmailAddress,
                                         PhoneNumber = "0000000000",
                                         State = "TX" // does this need to be dynamic?
                                     },
                                     Login = new PowerMenuSystemRequestAdminModel() {
                                         AdminUsername = Configuration.PowerMenuAdminUsername,
                                         AdminPassword = Configuration.PowerMenuAdminPassword
                                     },
                                     Operation = PowerMenuSystemRequestModel.Operations.Add
                                 }).First();


            return powerMenuRequest;
        }

		public void RemoveUserFromAccount(UserProfile removedBy, Guid accountId, Guid userId)
		{
            List<Account> allAccounts = _accountRepo.GetAccounts();
            Account acct = allAccounts.Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());

            AccountUsersReturn usersReturn = new AccountUsersReturn();
            usersReturn.AccountUserProfiles = _csProfile.GetUsersForCustomerOrAccount(accountId);
            usersReturn.CustomerUserProfiles = new List<UserProfile>();

			var user = GetUserProfile(userId);
			_extAd.RevokeAccess(removedBy.EmailAddress, user.UserProfiles[0].EmailAddress, ConvertRoleName(user.UserProfiles[0].RoleName));
			_extAd.GrantAccess(removedBy.EmailAddress, user.UserProfiles[0].EmailAddress, ConvertRoleName(Constants.ROLE_NAME_GUEST));

            foreach (Customer c in acct.Customers) {
                RemoveUserFromCustomer(removedBy, c.CustomerId, userId);
            }

			

            //Remove directly from account
            _accountRepo.RemoveUserFromAccount(removedBy.EmailAddress, accountId, userId);

			// remove the old user profile from cache and then update it with the new profile
			_cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(user.UserProfiles[0].EmailAddress));

        }

        public void RemoveUserFromCustomer(UserProfile removedBy, Guid customerId, Guid userId) {
            _customerRepo.RemoveUserFromCustomer(removedBy.EmailAddress, customerId, userId);
        }

        private void RequestKbitAccess(string emailAddress) {
            // create a new kbit app request
            KbitCustomerAccessRequest kbitAppRequest = new KbitCustomerAccessRequest();

            // get the user's profile
            UserProfileReturn userInfo = GetUserProfile(emailAddress, false);
            kbitAppRequest.UserName = userInfo.UserProfiles[0].UserName;

            // get all of the customers for the user
            List<Customer> customers = GetCustomersForExternalUser(userInfo.UserProfiles[0].UserId);

            // load all of the customers into the kbit app request
            kbitAppRequest.Customers = (from customer in customers
                                        select new UserSelectedContext() { BranchId = customer.CustomerBranch, CustomerId = customer.CustomerNumber }).ToList();

            _eventLog.WriteInformationLog(string.Format("Sending KBIT Authorization Request to the queue ({0}, {1}", emailAddress, kbitAppRequest.ToJSON()));

            // put the kbit app request in the queue
            SubmitExternalApplicationRequest(kbitAppRequest);
        }

        /// <summary>
        /// Reset a password administration style (no old password required)
        /// </summary>
        /// <param name="emailAddress"></param>
		public void ResetPassword(Guid userId, string newPassword){
			var user = GetUserProfile(userId);
			if (user.UserProfiles.Count != 1)
				return;
            
            _extAd.UpdatePassword(user.UserProfiles[0].EmailAddress, newPassword);
			_extAd.UnlockAccount(user.UserProfiles[0].EmailAddress);
        }

        /// <summary>
        /// revoke access in the customer AD for the user to the specified role and update the user cache
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="roleName">the requested role</param>
        /// <remarks>
        /// jwames - 3/10/2015 - original code
        /// </remarks>
        public void RevokeRoleAccess(UserProfile updatedBy, string emailAddress, AccessRequestType requestedApp) {
            string appRoleName;

            switch (requestedApp) {
                case AccessRequestType.KbitCustomer:
                    appRoleName = Configuration.AccessGroupKbitCustomer;
                    break;
                case AccessRequestType.PowerMenu:
                    appRoleName = Configuration.AccessGroupPowerMenuCustomer;
                    break;
                default:
                    return;
            }

            _extAd.RevokeAccess(updatedBy.EmailAddress, emailAddress, appRoleName);

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(emailAddress));

            switch (requestedApp) {
                case AccessRequestType.KbitCustomer:
                    RemoveKbitAccess(emailAddress);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Send Password Change emails with a specific template
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="newPassword"></param>
        /// <param name="MessageTemplate"></param>
        private void SendPasswordChangeEmail(string emailAddress, string newPassword, string MessageTemplate) {
            try {
                var template = _msgTemplateLogic.ReadForKey(MessageTemplate);
                if (template != null) {
                    _emailClient.SendTemplateEmail(template, new List<string>() { emailAddress }, new { password = newPassword, url = Configuration.PresentationUrl });
                } else {
                    throw new Exception(String.Format("Message template: {0} returned null. Message for new user creation could not be sent", MessageTemplate));
                };
            } catch (Exception ex) {
                _eventLog.WriteErrorLog("Error sending user profile email", ex);
            }
        }

        private void SetupPowerMenuForAccount(string emailAddress) {
            // get the users profile
            UserProfileReturn userInfo = GetUserProfile( emailAddress, false );

            // get all customers for the user
            List<Customer> customers = GetCustomersForExternalUser( userInfo.UserProfiles[0].UserId );

            // create a request for every powermenu customer
            PowerMenuSystemRequestModel powerMenuRequest = BuildPowerMenuRequest( customers, userInfo, PowerMenuSystemRequestModel.Operations.Add );

            //If there are customers to add, send the request to PowerMenu 
            SendPowerMenuRequests( powerMenuRequest, emailAddress );
        }

        /// <summary>
        /// Prepare and send the powermenu requests to the queue
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="emailAddress"></param>
        private void SendPowerMenuRequests( PowerMenuSystemRequestModel request,  string emailAddress ) {
            PowerMenuCustomerAccessRequest accessRequest = new PowerMenuCustomerAccessRequest();
            PowerMenuSystemRequestModel powerMenuRequest = new PowerMenuSystemRequestModel();

            accessRequest.UserName = emailAddress;
            accessRequest.RequestType = AccessRequestType.PowerMenu;
            accessRequest.request = request;

            SubmitExternalApplicationRequest( accessRequest );
        }

        /// <summary>
        /// send the external application request to the message queue 
        /// </summary>
        /// <param name="appRequest">the application request</param>
        /// <remarks>
        /// jwames - 3/12/2015 - original code
        /// </remarks>
        public void SubmitExternalApplicationRequest(BaseAccessRequest appRequest) {
            _queue.PublishToQueue(appRequest.ToJSON(), Configuration.RabbitMQAccessServer, Configuration.RabbitMQAccessUserNamePublisher,
                                  Configuration.RabbitMQAccessUserPasswordPublisher, Configuration.RabbitMQVHostAccess, Configuration.RabbitMQExchangeAccess);
        }

        /// <summary>
        /// Admin created user - sets a temporary password and expires it so they have to change it on next login
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="branchId"></param>
        /// <returns>Returns a new user profile</returns>
        /// <remarks>
        /// jwames - 4/2/2015 - change AD structure
        /// </remarks>
        public UserProfileReturn UserCreatedGuestWithTemporaryPassword(UserProfile actingUser, string emailAddress, string branchId) {
            if(ProfileHelper.IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot create an account in External AD for an Internal User"); }
            string generatedPassword = GenerateTemporaryPassword(); //This generated password is no longer being sent to the user, but it's still needed to create the account in AD

            AssertGuestProfile(emailAddress, generatedPassword);

            _extAd.CreateUser(
                Configuration.ActiveDirectoryExternalUserContainer,
                emailAddress,
                generatedPassword,
                Core.Constants.AD_GUEST_FIRSTNAME,
                Core.Constants.AD_GUEST_LASTNAME,
                Configuration.RoleNameGuest
                );

            _csProfile.CreateUserProfile(
               actingUser.EmailAddress,
                emailAddress,
                Core.Constants.AD_GUEST_FIRSTNAME,
                Core.Constants.AD_GUEST_LASTNAME,
                string.Empty,
                branchId
                );

            _passwordLogic.GenerateNewUserPasswordLink(emailAddress);


            return GetUserProfile(emailAddress);
        }

        public bool UpdateAccount(UserProfile updatedBy, Guid accountId, string name, List<Customer> customers, List<UserProfile> users) {
            List<Customer> existingCustomers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());
            List<UserProfile> existingUsers = _csProfile.GetUsersForCustomerOrAccount(accountId);

            IEnumerable<Guid> customersToAdd = customers.Select(c => c.CustomerId).Except(existingCustomers.Select(c => c.CustomerId));
            IEnumerable<Guid> customersToDelete = existingCustomers.Select(c => c.CustomerId).Except(customers.Select(c => c.CustomerId));
            IEnumerable<Guid> usersToAdd = users.Select(u => u.UserId).Except(existingUsers.Select(u => u.UserId));
            IEnumerable<Guid> usersToDelete = existingUsers.Select(u => u.UserId).Except(users.Select(u => u.UserId));

            //First make sure the customer is not already a member of an account
            foreach (Guid custId in customersToAdd) {
                var cust = _customerRepo.GetCustomerById(custId);
                if (cust != null)
                    if (cust.AccountId != null)
                        throw new Exception(string.Format("Customer {0}-{1} is already a member of a Customer Group", cust.CustomerNumber, cust.CustomerName));
            }



            foreach (Guid g in customersToAdd)
                _accountRepo.AddCustomerToAccount(updatedBy.EmailAddress, accountId, g);
            foreach (Guid g in customersToDelete)
                _accountRepo.RemoveCustomerFromAccount(updatedBy.EmailAddress, accountId, g);
            foreach (Guid g in usersToAdd)
                _accountRepo.AddUserToAccount(updatedBy.EmailAddress, accountId, g);
            foreach (Guid g in usersToDelete)
                _accountRepo.RemoveUserFromAccount(updatedBy.EmailAddress, accountId, g);

            // update account user roles to owner
            foreach (UserProfile user in users) // all account users are assumed to be owners on all customers
                UpdateCustomersForUser(updatedBy, customers, user.RoleName, user);

            _accountRepo.UpdateAccount(name, accountId);

            // refresh cache; need to reload customers
            _customerRepo.ClearCustomerCache();
            return true;
        }

        private void UpdateCustomersForUser(UserProfile updatedBy, List<Customer> customerList, string roleName, UserProfile existingUser) {
            var customers = GetNonPagedCustomersForUser(existingUser);

            IEnumerable<Guid> custsToAdd = customerList.Select(c => c.CustomerId).Except(customers.Select(b => b.CustomerId));
            IEnumerable<Guid> custsToRemove = customers.Select(b => b.CustomerId).Except(customerList.Select(c => c.CustomerId));
            foreach (Guid c in custsToAdd)
                AddUserToCustomer(updatedBy, c, existingUser.UserId);
            foreach (Guid c in custsToRemove)
				RemoveUserFromCustomer(updatedBy, c, existingUser.UserId);
            //UpdateUserRoles(customerList.Select(x => x.CustomerName).ToList(), existingUser.EmailAddress, roleName);
            if (!roleName.Equals(existingUser.RoleName, StringComparison.InvariantCultureIgnoreCase)) {
                _extAd.GrantAccess(updatedBy.EmailAddress, existingUser.EmailAddress, ConvertRoleName(roleName));
                _extAd.RevokeAccess(updatedBy.EmailAddress, existingUser.EmailAddress, ConvertRoleName(existingUser.RoleName));
            }
        }

        /// <summary>
        /// make sure that the new password meets our requirements, the old password is correct, and updates saves the new password
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="originalPassword">the user's old password </param>
        /// <param name="newPassword">the user's new password</param>
        /// <returns>message detailing the results</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public bool UpdateUserPassword(UserProfile updatedBy, string emailAddress, string originalPassword, string newPassword) {
            bool retVal = false;

            if (ProfileHelper.IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot change password for BEK user"); }

            UserProfile existingUser = GetUserProfile(emailAddress).UserProfiles[0];

            AssertPasswordLength(newPassword);
            AssertPasswordComplexity(newPassword);
            AssertPasswordVsAttributes(newPassword, existingUser.FirstName, existingUser.LastName);

            if (_extAd.UpdatePassword(updatedBy.EmailAddress, emailAddress, originalPassword, newPassword)) {
                retVal = true;
            } else {
                throw new ApplicationException("Password was invalid");
            }

            return retVal;
        }

        /// <summary>
        /// update the user profile in Commerce Server (not implemented)
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
		public void UpdateUserProfile(UserProfile updatedBy, Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId,
            bool updateCustomerListAndRole, List<Customer> customerList, string roleName) {
            AssertEmailAddressLength(emailAddress);
            AssertEmailAddress(emailAddress);
            AssertFirstNameLength(firstName);
            AssertLastNameLength(lastName);

            UserProfileReturn existingUser = GetUserProfile(id);

            if (string.Compare(existingUser.UserProfiles[0].EmailAddress, emailAddress, true) != 0) { AssertEmailAddressUnique(emailAddress); }

            if (ProfileHelper.IsInternalAddress(emailAddress) || ProfileHelper.IsInternalAddress(existingUser.UserProfiles[0].EmailAddress)) {
                throw new ApplicationException("Cannot update profile information for BEK user.");
            }

            _csProfile.UpdateUserProfile(updatedBy.EmailAddress, id, emailAddress, firstName, lastName, phoneNumber, branchId);

            _extAd.UpdateUserAttributes(existingUser.UserProfiles[0].EmailAddress, emailAddress, firstName, lastName);
			
            // update customer list
            if (updateCustomerListAndRole && customerList != null && customerList.Count > 0) {
                UpdateCustomersForUser(updatedBy, customerList, roleName, existingUser.UserProfiles[0]);
            }

            // remove the old user profile from cache and then update it with the new profile
            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(existingUser.UserProfiles[0].EmailAddress));

        }

        //public void UpdateUserRoles(List<string> customerNames, string emailAddress, string roleName)
        //{
        //    _extAd.UpdateUserGroups(customerNames, roleName, emailAddress);
        //}

        public List<SettingsModelReturn> GetProfileSettings( Guid userId ) {
            return _settingsLogic.GetAllUserSettings( userId );
        }

        public void SaveProfileSettings( SettingsModel model ) {
            _settingsLogic.CreateOrUpdateSettings( model );
        }

        public void DeleteProfileSettings(SettingsModel model)
        {
            _settingsLogic.DeleteSettings( model );
        }

        public void SetUserProfileLastLogin(Guid id)
        {
            _csProfile.UpdateUserProfileLastLogin(id);
        }

        public void SetUserProfileLastAccess(Guid id)
        {
            _csProfile.UpdateUserProfileLastAccess(id);
        }
        /// <summary>
        /// UNFI Whitelisting configurations - these are temporary entries
        /// </summary>
        /// <param name="user">the userprofile of who's logged in</param>
        /// <remarks>
        /// bakillins - 1/22/2016
        /// </remarks>
        /// <returns>a bool that's true if they are on the whitelists</returns>
        public bool CheckCanViewUNFI(UserProfile user, string customernumber)
        {
            if ((user != null) &&
                (user.IsInternalUser) &&
                (KeithLink.Svc.Impl.Configuration.WhiteListedUNFIBEKUsers.Contains(user.UserName, StringComparer.CurrentCultureIgnoreCase)))
                return true;
            if ((user != null) &&
                (user.RoleName.Equals("dsr", StringComparison.CurrentCultureIgnoreCase)) &&
                (KeithLink.Svc.Impl.Configuration.WhiteListedUNFIDSRs.Contains(user.UserName, StringComparer.CurrentCultureIgnoreCase)))
                return true;
            if ((customernumber != null) &&
                (KeithLink.Svc.Impl.Configuration.WhiteListedUNFICustomers.Contains(customernumber)))
                return true;
            return false;
        }
        #endregion
	}
}
