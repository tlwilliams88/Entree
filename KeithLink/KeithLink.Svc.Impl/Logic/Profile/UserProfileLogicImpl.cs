using CommerceServer.Foundation;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.SingleSignOn;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.SingleSignOn;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Linq;
using System.Text.RegularExpressions;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class UserProfileLogicImpl : IUserProfileLogic {
        #region attributes
        private const string GUEST_USER_WELCOME = "GuestUserWelcome";
        private const string CREATED_USER_WELCOME = "CreatedUserWelcome";
        private const string RESET_PASSWORD = "ResetPassword";

		protected string CACHE_GROUPNAME { get { return "Profile"; } }
		protected string CACHE_NAME { get { return "Profile"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }


        private ICacheRepository _cache;
        private IUserProfileRepository _csProfile;
        private ICustomerDomainRepository _extAd;
        private IUserDomainRepository _intAd;
        private IAccountRepository _accountRepo;
        private ICustomerRepository _customerRepo;
		private IOrderServiceRepository _orderServiceRepository;
        private IMessagingServiceRepository _msgServiceRepo;
		private IInvoiceServiceRepository _invoiceServiceRepository;
		private IEmailClient _emailClient;
		private IMessagingServiceRepository _messagingServiceRepository;
		private IEventLogRepository _eventLog;
		private IOnlinePaymentServiceRepository _onlinePaymentServiceRepository;
        private IGenericQueueRepository _queue;
        #endregion

        #region ctor
        public UserProfileLogicImpl(ICustomerDomainRepository externalAdRepo, IUserDomainRepository internalAdRepo, IUserProfileRepository commerceServerProfileRepo,
									ICacheRepository profileCache, IAccountRepository accountRepo, ICustomerRepository customerRepo, IOrderServiceRepository orderServiceRepository,
									IMessagingServiceRepository msgServiceRepo, IInvoiceServiceRepository invoiceServiceRepository, IEmailClient emailClient, IMessagingServiceRepository messagingServiceRepository,
									IEventLogRepository eventLog, IOnlinePaymentServiceRepository onlinePaymentServiceRepository, IGenericQueueRepository queue)
		{
            _cache = profileCache;
            _extAd = externalAdRepo;
            _intAd = internalAdRepo;
            _csProfile = commerceServerProfileRepo;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
			_orderServiceRepository = orderServiceRepository;
            _msgServiceRepo = msgServiceRepo;
			_invoiceServiceRepository = invoiceServiceRepository;
			_emailClient = emailClient;
			_messagingServiceRepository = messagingServiceRepository;
			_eventLog = eventLog;
			_onlinePaymentServiceRepository = onlinePaymentServiceRepository;
            _queue = queue;
        }
        #endregion

        #region methods
        public void AddCustomerToAccount(Guid accountId, Guid customerId) {
            _accountRepo.AddCustomerToAccount(accountId, customerId);
        }

        public string AddProfileImageUrl(Guid userId) {
            return String.Format("{0}{1}{2}", Configuration.MultiDocsProxyUrl, "avatar/", userId);
        }

        public void AddUserToCustomer(Guid customerId, Guid userId, string role) {
            // TODO: Create user if they don't exist....   Add ROLE to call
            _accountRepo.AddCustomerToAccount(customerId, userId);
        }

        public void AddUserToCustomer(Guid customerId, Guid userId) {
            _customerRepo.AddUserToCustomer(customerId, userId);
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

        private string CacheKey(string email) {
            return string.Format("{0}-{1}", "bek", email);
        }

        public AccountReturn CreateAccount(string name) {
            // call CS account repository -- hard code it for now
            Guid newAcctId = _accountRepo.CreateAccount(name);
            return new AccountReturn() { Accounts = new List<Account>() { new Account() { Name = name, Id = newAcctId } } };
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

            _csProfile.CreateUserProfile(emailAddress, fName, bekUser.Surname, bekUser.GetPhoneNumber(), GetBranchFromOU(bekUser.GetOrganizationalunit()));
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
        /// </remarks>
        public UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId) {
            if (emailAddress == null) throw new Exception( "email address cannot be null" );
            if (password == null) throw new Exception( "password cannot be null" );

            AssertGuestProfile(emailAddress, password);

            _extAd.CreateUser(Configuration.ActiveDirectoryGuestContainer,
                              emailAddress, 
                              password, 
                              Core.Constants.AD_GUEST_FIRSTNAME, 
                              Core.Constants.AD_GUEST_LASTNAME, 
                              Core.Constants.ROLE_EXTERNAL_GUEST
                              );

            _csProfile.CreateUserProfile(emailAddress,
                                         Core.Constants.AD_GUEST_FIRSTNAME,
                                         Core.Constants.AD_GUEST_LASTNAME,
                                         string.Empty,
                                         branchId
                                         );

                try {
                    var template = _messagingServiceRepository.ReadMessageTemplateForKey( GUEST_USER_WELCOME );

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
        public UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId) {
            AssertUserProfile(customerName, emailAddress, password, firstName, lastName, phone, roleName);

            _extAd.CreateUser(customerName,
                              emailAddress,
                              password,
                              firstName,
                              lastName,
                              roleName
                              );

            _csProfile.CreateUserProfile(emailAddress,
                                         firstName,
                                         lastName,
                                         phone,
                                         branchId
                                         );

            return GetUserProfile(emailAddress);
        }

        public Core.Models.Paging.PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, Core.Models.Paging.PagingModel paging, string account) {
            if (string.IsNullOrEmpty(searchTerms))
                searchTerms = "";

            if (!string.IsNullOrEmpty(account))
                return _customerRepo.GetPagedCustomersForAccount(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, searchTerms, account.ToGuid().ToCommerceServerFormat());


            if (IsInternalAddress(user.EmailAddress)) {
                if (user.IsDSR && !String.IsNullOrEmpty(user.DSRNumber)) {
                    // lookup customers by their assigned dsr number
                    return _customerRepo.GetPagedCustomersForDSR(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, user.DSRNumber, user.BranchId, searchTerms);

                }
                if (user.IsDSM && !String.IsNullOrEmpty(user.DSMNumber)) {
                    // lookup customers by their assigned dsr number
                    return _customerRepo.GetPagedCustomersForDSR(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, user.DSMNumber, user.BranchId, searchTerms);

                } else if (user.RoleName == "branchismanager") {
                    return _customerRepo.GetPagedCustomersForBranch(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, user.BranchId, searchTerms);

                } else { // assume admin user with access to all customers
                    return _customerRepo.GetPagedCustomers(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, searchTerms);
                }
            } else // external user
			{
                return _customerRepo.GetPagedCustomersForUser(paging.Size.HasValue ? paging.Size.Value : int.MaxValue, paging.From.HasValue ? paging.From.Value : 0, user.UserId, searchTerms);
            }
        }

        /// <summary>
        /// take all of the fields from the commerce server profile and put them into our custom object and load other custom data
        /// </summary>
        /// <param name="csProfile">profile data from commerce server</param>
        /// <returns>a completed user profile object</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - derived from CombineCSAndADProfile method
        /// </remarks>
        public UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false) {
            //List<Customer> userCustomers;
            string dsrRole = string.Empty;
            string dsrNumber = string.Empty;
            string dsmRole = string.Empty;
            string userRole = string.Empty;
            string userBranch = string.Empty;
            bool isInternalUser = IsInternalAddress(csProfile.Email);
            UserPrincipal adUser = null;
            bool isKbitCustomer = false;

            if (isInternalUser) {
                adUser = _intAd.GetUser(csProfile.Email);
                DirectoryEntry directoryEntry = (DirectoryEntry)adUser.GetUnderlyingObject();
                List<string> internalUserRoles = _intAd.GetAllGroupsUserBelongsTo(adUser, Svc.Core.Constants.INTERNAL_USER_ROLES);

                if (internalUserRoles.Intersect(Constants.BEK_SYSADMIN_ROLES).Count() > 0) {
                    userRole = "beksysadmin";
                } else if (internalUserRoles.Intersect(Constants.MIS_ROLES).Count() > 0) {
                    userRole = "branchismanager";
                    userBranch = internalUserRoles.Intersect(Constants.MIS_ROLES).FirstOrDefault().ToString().Substring(0, 3);
                } else if (internalUserRoles.Intersect(Constants.DSM_ROLES).Count() > 0) {
                    dsmRole = internalUserRoles.Intersect(Constants.DSM_ROLES).FirstOrDefault().ToString();
                    userRole = "dsm";
                    userBranch = dsmRole.Substring(0, 3);
                    dsrNumber = StringExtensions.ToInt(adUser.Description) != null ? adUser.Description : string.Empty;
                } else if (internalUserRoles.Intersect(Constants.DSR_ROLES).Count() > 0) {
                    dsrRole = internalUserRoles.Intersect(Constants.DSR_ROLES).FirstOrDefault().ToString();
                    userRole = "dsr";
                    dsrNumber = StringExtensions.ToInt(adUser.Description) != null ? adUser.Description : string.Empty;
                    userBranch = dsrRole.Substring(0, 3);
                } else {
                    userRole = "guest";
                }

            } else {
                adUser = _extAd.GetUser(csProfile.Email);
                userRole = GetUserRole(csProfile.Email);
                userBranch = csProfile.DefaultBranch;
                isKbitCustomer = _extAd.HasAccess(csProfile.Email, Configuration.AccessGroupKbitCustomer);
            }

            string userNameToken = string.Concat(adUser.SamAccountName, "-", DateTime.Now.ToString("yyyyMMddHHmmss"));
            byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(userNameToken);
            string tokenBase64 = Convert.ToBase64String(tokenBytes);

            return new UserProfile() {
                UserId = Guid.Parse(csProfile.Id),
                IsInternalUser = IsInternalAddress(csProfile.Email),
                PasswordExpired = (isInternalUser) ? false : _extAd.IsPasswordExpired(csProfile.Email),
                FirstName = csProfile.FirstName,
                LastName = csProfile.LastName,
                EmailAddress = csProfile.Email,
                PhoneNumber = csProfile.Telephone,
                CustomerNumber = csProfile.DefaultCustomer,
                BranchId = userBranch,
                RoleName = userRole,
                DSMRole = dsmRole,
                DSRNumber = dsrNumber,
                //UserCustomers = userCustomers,
                ImageUrl = AddProfileImageUrl(Guid.Parse(csProfile.Id)),
                UserName = adUser.SamAccountName,
                UserNameToken = tokenBase64,
                IsKBITCustomer = isKbitCustomer
            };
        }

        private string GenerateTemporaryPassword() {
            string generatedPassword = NewPassword();

            for (int i = 0; !PasswordMeetsComplexityRequirements(generatedPassword) && i < 8; i++)
                generatedPassword = NewPassword();

            return generatedPassword;
        }

        public AccountReturn GetAccounts(AccountFilterModel accountFilters) {
            List<Account> retAccounts = new List<Account>();

            if (accountFilters != null) {
                if (accountFilters.UserId.HasValue) {
                    retAccounts.AddRange(_accountRepo.GetAccountsForUser(accountFilters.UserId.Value));

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

        public Account GetAccount(Guid accountId) {
            Account acct = _accountRepo.GetAccounts().Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());
            acct.AdminUsers = _csProfile.GetUsersForCustomerOrAccount(accountId);
            acct.CustomerUsers = new List<UserProfile>();
            foreach (Customer c in acct.Customers) {
                acct.CustomerUsers.AddRange(_csProfile.GetUsersForCustomerOrAccount(c.CustomerId));
            }
            acct.CustomerUsers = acct.CustomerUsers
                                    .GroupBy(x => x.UserId)
                                    .Select(grp => grp.First())
                                    .ToList();

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

            returnModel.LastOrderUpdate = _orderServiceRepository.ReadLatestUpdatedDate(new Core.Models.SiteCatalog.UserSelectedContext() { BranchId = branchId, CustomerId = customerId });
            returnModel.balance = _onlinePaymentServiceRepository.GetCustomerAccountBalance(customerId, branchId);

            return returnModel;
        }

        /// <summary>
        /// translate the AD branch name to mainframe branch name
        /// </summary>
        /// <param name="OU">user's organization unit</param>
        /// <returns>3-digit branch name</returns>
        /// <remarks>
        /// jwames - 10/9/2014 - original code
        /// </remarks>
        private string GetBranchFromOU(string OU) {
            switch (OU) {
                case "FABQ":
                    return "FAQ";
                case "FAMA":
                case "FDFW":
                case "FHST":
                case "FLRK":
                case "FOKC":
                case "FSAN":
                    return OU.Substring(0, 3);
                default:
                    return null;
            }
        }

        public Customer GetCustomerByCustomerNumber(string customerNumber, string branchId) {
            return _customerRepo.GetCustomerByCustomerNumber(customerNumber, branchId);
        }

        public List<Customer> GetCustomersForExternalUser(Guid userId) {
            Core.Models.Generated.UserProfile profile = _csProfile.GetCSProfile(userId);

            if (IsInternalAddress(profile.Email)) {
                throw new ApplicationException("This call is not supported for internal users.");
            } else {
                return _customerRepo.GetCustomersForUser(userId);
            }
        }

        public Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId) {
            return _customerRepo.GetCustomerForUser(customerNumber, branchId, userId);
        }

        public List<ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid) {
            var currentMessagingPreferences = _msgServiceRepo.ReadMessagingPreferences(guid);
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

        public List<Customer> GetNonPagedCustomersForUser(UserProfile user, string search = "") {
            List<Customer> allCustomers = new List<Customer>();
            if (string.IsNullOrEmpty(search))
                search = "";
            if (IsInternalAddress(user.EmailAddress)) {
                if (!String.IsNullOrEmpty(user.DSRNumber)) {
                    // lookup customers by their assigned dsr number
                    allCustomers = _customerRepo.GetCustomersForDSR(user.DSRNumber, user.BranchId);
                } else if (!String.IsNullOrEmpty(user.DSMRole)) {
                    //lookup customers by their assigned dsm number
                    _customerRepo.GetCustomersForDSM(user.DSMNumber, user.BranchId);
                } else if (user.RoleName == "branchismanager") {
                    if (search.Length >= 3)
                        allCustomers = _customerRepo.GetCustomersByNameSearchAndBranch(search, user.BranchId);
                    else
                        allCustomers = _customerRepo.GetCustomersForUser(user.UserId);
                } else { // assume admin user with access to all customers
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

        public PagedResults<Account> GetPagedAccounts(PagingModel paging) {
            var accounts = _accountRepo.GetAccounts();

            return accounts.AsQueryable().GetPage(paging);
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

            if (IsInternalAddress(emailAddress).Equals(false) && profile != null) {
                profile.PasswordExpired = _extAd.IsPasswordExpired(emailAddress); // always check password expired status; even when cached...
            }

            if (profile != null) {
                retVal.UserProfiles.Add(profile);
                return retVal;
            }

            // search commerce server next
            Core.Models.Generated.UserProfile csUserProfile = _csProfile.GetCSProfile(emailAddress);

            if (csUserProfile == null) {
                if (IsInternalAddress(emailAddress)) {
                    CreateBekUserProfile(emailAddress);

                    return GetUserProfile(emailAddress);
                }
            } else {
                retVal.UserProfiles.Add(FillUserProfile(csUserProfile, includeTermInformation: true));
            }

            // add to cache if found
            if (retVal.UserProfiles.Count > 0) {
                _cache.AddItem<UserProfile>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(retVal.UserProfiles.FirstOrDefault().EmailAddress), TimeSpan.FromHours(2), retVal.UserProfiles.FirstOrDefault());
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
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Id"] = UserId.ToString();
            profileQuery.SearchCriteria.Model.DateModified = DateTime.Now;

            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("SelectedBranch");
            profileQuery.Model.Properties.Add("SelectedCustomer");
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

            if (IsInternalAddress(email)) {
                //roleName = _intAd.
                roleName = "owner";
            } else {
                roleName = _extAd.GetUserGroup(email, new List<string>() { "owner", "approver", "buyer", "accounting", "guest" });
            }

            return roleName;
        }

        public UserProfileReturn GetUsers(UserFilterModel userFilters) {
            if (userFilters != null) {
                if (userFilters.AccountId.HasValue) {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.AccountId.Value) };
                } else if (userFilters.CustomerId.HasValue) {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.CustomerId.Value) };
                } else if (!String.IsNullOrEmpty(userFilters.Email)) {
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
        public void GrantRoleAccess(string emailAddress, RequestedApplication requestedApp) {
            string appRoleName;

            switch (requestedApp) {
                case RequestedApplication.KbitCustomer:
                    appRoleName = Configuration.AccessGroupKbitCustomer;
                    break;
                default:
                    return;
            }

            _extAd.GrantAccess(emailAddress, appRoleName);

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(emailAddress));

            switch (requestedApp) {
                case RequestedApplication.KbitCustomer:
                    RequestKbitAccess(emailAddress);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// looks for a benekeith.com email domain
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>true if found</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public bool IsInternalAddress(string emailAddress) {
            return Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS);
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

        public void RemoveUserFromAccount(Guid accountId, Guid userId) {
            List<Account> allAccounts = _accountRepo.GetAccounts();
            Account acct = allAccounts.Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomersForAccount(accountId.ToCommerceServerFormat());

            AccountUsersReturn usersReturn = new AccountUsersReturn();
            usersReturn.AccountUserProfiles = _csProfile.GetUsersForCustomerOrAccount(accountId);
            usersReturn.CustomerUserProfiles = new List<UserProfile>();

            foreach (Customer c in acct.Customers) {
                RemoveUserFromCustomer(c.CustomerId, userId);
            }

            //Remove directly from account
            _accountRepo.RemoveUserFromAccount(accountId, userId);
        }

        public void RemoveUserFromCustomer(Guid customerId, Guid userId) {
            _customerRepo.RemoveUserFromCustomer(customerId, userId);
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

            // put the kbit app request in the queue
            SubmitExternalApplicationRequest(kbitAppRequest);
        }

        /// <summary>
        /// Reset a password administration style (no old password required)
        /// </summary>
        /// <param name="emailAddress"></param>
        public void ResetPassword(string emailAddress) {
            if (emailAddress == null)
                throw new ArgumentException("EmailAddress cannot be null");

            string generatedPassword = GenerateTemporaryPassword();

            _extAd.UpdatePassword(emailAddress, generatedPassword);
            _extAd.ExpirePassword(emailAddress);

            SendPasswordChangeEmail(emailAddress, generatedPassword, RESET_PASSWORD);
        }

        /// <summary>
        /// revoke access in the customer AD for the user to the specified role and update the user cache
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="roleName">the requested role</param>
        /// <remarks>
        /// jwames - 3/10/2015 - original code
        /// </remarks>
        public void RevokeRoleAccess(string emailAddress, RequestedApplication requestedApp) {
            string appRoleName;

            switch (requestedApp) {
                case RequestedApplication.KbitCustomer:
                    appRoleName = Configuration.AccessGroupKbitCustomer;
                    break;
                default:
                    return;
            }

            _extAd.RevokeAccess(emailAddress, appRoleName);

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(emailAddress));

            switch (requestedApp) {
                case RequestedApplication.KbitCustomer:
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
                var template = _messagingServiceRepository.ReadMessageTemplateForKey(MessageTemplate);
                if (template != null) {
                    _emailClient.SendTemplateEmail(template, new List<string>() { emailAddress }, new { password = newPassword, url = Configuration.PresentationUrl });
                } else {
                    throw new Exception(String.Format("Message template: {0} returned null. Message for new user creation could not be sent", MessageTemplate));
                };
            } catch (Exception ex) {
                _eventLog.WriteErrorLog("Error sending user profile email", ex);
            }
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
        public UserProfileReturn UserCreatedGuestWithTemporaryPassword( string emailAddress, string branchId ) {
            string generatedPassword = GenerateTemporaryPassword();

            AssertGuestProfile( emailAddress, generatedPassword );

            _extAd.CreateUser(
                Configuration.ActiveDirectoryGuestContainer,
                emailAddress,
                generatedPassword,
                Core.Constants.AD_GUEST_FIRSTNAME,
                Core.Constants.AD_GUEST_LASTNAME,
                Core.Constants.ROLE_EXTERNAL_GUEST
                );

            _csProfile.CreateUserProfile(
                emailAddress,
                Core.Constants.AD_GUEST_FIRSTNAME,
                Core.Constants.AD_GUEST_LASTNAME,
                string.Empty,
                branchId
                );

            // Expire the users password so they can change it at neck login
            _extAd.ExpirePassword( emailAddress );
            SendPasswordChangeEmail( emailAddress, generatedPassword, CREATED_USER_WELCOME );

            return GetUserProfile( emailAddress );
        }

        public bool UpdateAccount(Guid accountId, string name, List<Customer> customers, List<UserProfile> users) {
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
                _accountRepo.AddCustomerToAccount(accountId, g);
            foreach (Guid g in customersToDelete)
                _accountRepo.RemoveCustomerFromAccount(accountId, g);
            foreach (Guid g in usersToAdd)
                _accountRepo.AddUserToAccount(accountId, g);
            foreach (Guid g in usersToDelete)
                _accountRepo.RemoveUserFromAccount(accountId, g);

            // update account user roles to owner
            foreach (UserProfile user in users) // all account users are assumed to be owners on all customers
                UpdateCustomersForUser(customers, "owner", user);

            _accountRepo.UpdateAccount(name, accountId);

            // refresh cache; need to reload customers
            _customerRepo.ClearCustomerCache();
            return true;
        }

        private void UpdateCustomersForUser(List<Customer> customerList, string roleName, UserProfile existingUser) {
            var customers = GetNonPagedCustomersForUser(existingUser);

            IEnumerable<Guid> custsToAdd = customerList.Select(c => c.CustomerId).Except(customers.Select(b => b.CustomerId));
            IEnumerable<Guid> custsToRemove = customers.Select(b => b.CustomerId).Except(customerList.Select(c => c.CustomerId));
            foreach (Guid c in custsToAdd)
                AddUserToCustomer(c, existingUser.UserId);
            foreach (Guid c in custsToRemove)
                RemoveUserFromCustomer(c, existingUser.UserId);
            UpdateUserRoles(customerList.Select(x => x.CustomerName).ToList(), existingUser.EmailAddress, roleName);
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
        public bool UpdateUserPassword(string emailAddress, string originalPassword, string newPassword) {
            bool retVal = false;

            if (IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot change password for BEK user"); }

            UserProfile existingUser = GetUserProfile(emailAddress).UserProfiles[0];

            AssertPasswordLength(newPassword);
            AssertPasswordComplexity(newPassword);
            AssertPasswordVsAttributes(newPassword, existingUser.FirstName, existingUser.LastName);

            if (_extAd.UpdatePassword(emailAddress, originalPassword, newPassword)) {
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
        public void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId,
            bool updateCustomerListAndRole, List<Customer> customerList, string roleName) {
            AssertEmailAddressLength(emailAddress);
            AssertEmailAddress(emailAddress);
            AssertFirstNameLength(firstName);
            AssertLastNameLength(lastName);

            UserProfileReturn existingUser = GetUserProfile(id);

            if (string.Compare(existingUser.UserProfiles[0].EmailAddress, emailAddress, true) != 0) { AssertEmailAddressUnique(emailAddress); }

            if (IsInternalAddress(emailAddress) || IsInternalAddress(existingUser.UserProfiles[0].EmailAddress)) {
                throw new ApplicationException("Cannot update profile information for BEK user.");
            }

            _csProfile.UpdateUserProfile(id, emailAddress, firstName, lastName, phoneNumber, branchId);

            _extAd.UpdateUserAttributes(existingUser.UserProfiles[0].EmailAddress, emailAddress, firstName, lastName);

            // update customer list
            if (updateCustomerListAndRole && customerList != null && customerList.Count > 0) {
                UpdateCustomersForUser(customerList, roleName, existingUser.UserProfiles[0]);
            }

            // remove the old user profile from cache and then update it with the new profile
            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CacheKey(existingUser.UserProfiles[0].EmailAddress));

        }

        public void UpdateUserRoles(List<string> customerNames, string emailAddress, string roleName)
        {
            _extAd.UpdateUserGroups(customerNames, roleName, emailAddress);
        }

        #endregion
    }
}
