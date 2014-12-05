using CommerceServer.Foundation;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text.RegularExpressions;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class UserProfileLogicImpl : IUserProfileLogic {
        #region attributes
        private IUserProfileCacheRepository _cache;
        private IUserProfileRepository      _csProfile;
        private ICustomerDomainRepository   _extAd;
        private IUserDomainRepository       _intAd;
        private IAccountRepository _accountRepo;
        private ICustomerRepository _customerRepo;
		private IOrderServiceRepository _orderServiceRepository;
        private IMessagingServiceRepository _msgServiceRepo;
		private IInvoiceServiceRepository _invoiceServiceRepository;
		private IEmailClient _emailClient;
		private IMessagingServiceRepository _messagingServiceRepository;
		private IEventLogRepository _eventLog;
        #endregion

        #region ctor
        public UserProfileLogicImpl(ICustomerDomainRepository externalAdRepo, IUserDomainRepository internalAdRepo, IUserProfileRepository commerceServerProfileRepo, 
                                    IUserProfileCacheRepository profileCache, IAccountRepository accountRepo, ICustomerRepository customerRepo, IOrderServiceRepository orderServiceRepository,
									IMessagingServiceRepository msgServiceRepo, IInvoiceServiceRepository invoiceServiceRepository, IEmailClient emailClient, IMessagingServiceRepository messagingServiceRepository,
									IEventLogRepository eventLog)
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
        }
        #endregion

        #region methods

        public void AddUserToCustomer(Guid customerId, Guid userId, string role) {
            // TODO: Create user if they don't exist....   Add ROLE to call
            _accountRepo.AddCustomerToAccount(customerId, userId);
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
            if (System.Text.RegularExpressions.Regex.IsMatch(password, Core.Constants.REGEX_PASSWORD_PATTERN) == false) {
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

        public AccountReturn CreateAccount(string name) {
            // call CS account repository -- hard code it for now
            Guid newAcctId = _accountRepo.CreateAccount(name);
            return new AccountReturn() { Accounts = new List<Account>() { new Account() { Name = name, Id = newAcctId } } };
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
        public UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId, bool allowPasswordGeneration = false) {
            // if password is null or empty, create a temproary password and email it to the user
            bool generatedPassword = false;
            if (String.IsNullOrEmpty(password) && allowPasswordGeneration)
            {
                generatedPassword = true;
				password = System.Web.Security.Membership.GeneratePassword(8, 0);
            }
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

			if (generatedPassword) //TODO: Switch to new email client with message templates
				KeithLink.Common.Core.Email.NewUserEmail.Send(emailAddress, "Welcome to Entree.  Please use this temporary password to access Entree.\r\nPassword: " + password + "\r\nURL: https://shopqa.benekeith.com");
			else
			{
				try
				{
					var template = _messagingServiceRepository.ReadMessageTemplateForKey("GuestUserWelcome");

					if (template != null)
						_emailClient.SendTemplateEmail(template, new List<string>() { emailAddress }, null, null, new { contactEmail = Configuration.BranchContactEmail(branchId) });
				}
				catch (Exception ex)
				{
					//The registration probably shouldn't fail just because of an SMTP issue. So ignore this error and log
					_eventLog.WriteErrorLog("Error sending welcome email", ex);
				}
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

        /// <summary>
        /// take all of the fields from the commerce server profile and put them into our custom object and load other custom data
        /// </summary>
        /// <param name="csProfile">profile data from commerce server</param>
        /// <returns>a completed user profile object</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - derived from CombineCSAndADProfile method
        /// </remarks>
		public UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false)
		{
            List<Customer> userCustomers;
			string dsrRole = string.Empty;
            
            if (IsInternalAddress(csProfile.Email))
            {
                UserPrincipal user = _intAd.GetUser(csProfile.Email);
                dsrRole = GetUserDsrRole(user);
                if (!String.IsNullOrEmpty(dsrRole))
                {
                    // lookup customers by their assigned dsr number
                    userCustomers = _customerRepo.GetCustomers().Where(x => x.DsrNumber == user.Description).OrderBy(x => x.CustomerName).ToList();
                }
                else
                {
                    string dsmRole = GetUserDsmRole(user);
                    if (!String.IsNullOrEmpty(dsmRole))
                    {
                        // lookup customers by DSM; by looking at their DSR's - how to look at their DSRs?
                        userCustomers = _customerRepo.GetCustomers().OrderBy(x => x.CustomerName).ToList(); // TODO: reduce list to only the DSM's DSRs
                    }
                    else
                    {
                        // until we add customer service logic, return all customers for non-DSM/non-DSR internal users.  certain things will be missing (contract lists) if there is no account yet...
                        //userCustomers = _customerRepo.GetCustomers().OrderBy(x => x.CustomerName).ToList();
                        userCustomers = _customerRepo.GetCustomersForUser(Guid.Parse(csProfile.Id)).OrderBy(x => x.CustomerName).ToList(); //use the use the user organization object for customer filtering
                    }
                }
            }
            else
            {
                 userCustomers = _customerRepo.GetCustomersForUser(Guid.Parse(csProfile.Id)).OrderBy(x => x.CustomerName).ToList();
            }

            if (includeLastOrderDate)
            {
                //Populate the Last order updated date for each customer
                foreach (var customer in userCustomers)
                    customer.LastOrderUpdate = _orderServiceRepository.ReadLatestUpdatedDate(new Core.Models.SiteCatalog.UserSelectedContext() { BranchId = customer.CustomerBranch, CustomerId = customer.CustomerNumber });
            }
            
			if (includeTermInformation)
			{
				foreach (var cust in userCustomers)
				{
					if (string.IsNullOrEmpty(cust.TermCode))
						continue;

					//Lookup Term info
					var term = _invoiceServiceRepository.ReadTermInformation(cust.CustomerBranch, cust.TermCode);

					if (term != null)
					{
						cust.TermDescription = term.Description;
						cust.BalanceAge1Label = string.Format("0 - {0}", term.Age1);
						cust.BalanceAge2Label = string.Format("{0} - {1}", term.Age1, term.Age2);
						cust.BalanceAge3Label = string.Format("{0} - {1}", term.Age2, term.Age3);
						cust.BalanceAge4Label = string.Format("Over {0}", term.Age4);
					}

				}
			}
            

            return new UserProfile() {
                UserId = Guid.Parse(csProfile.Id),
                FirstName = csProfile.FirstName,
                LastName = csProfile.LastName,
                EmailAddress = csProfile.Email,
                PhoneNumber = csProfile.Telephone,
                CustomerNumber = csProfile.DefaultCustomer,
                BranchId = csProfile.DefaultBranch,
                RoleName = !String.IsNullOrEmpty(dsrRole) ? "dsr" : GetUserRole(csProfile.Email),
                UserCustomers = userCustomers,
                //new List<Customer>() { // for testing only
                                //        new Customer() { CustomerName = "Bob's Crab Shack", CustomerNumber = "709333", CustomerBranch = "fdf" },
                                //        new Customer() { CustomerName = "Julie's Taco Cabana", CustomerNumber = "709333", CustomerBranch = "fdf" }
                //}
                MessagingPreferences = GetMessagingPreferences(Guid.Parse(csProfile.Id))
            };
        }

        private List<ProfileMessagingPreferenceDetailModel> BuildPreferenceModelForEachNotificationType(List<UserMessagingPreferenceModel> currentMsgPrefs, string customerNumber)
        {
            var msgPrefModelList = new List<ProfileMessagingPreferenceDetailModel>();
            //loop through each notification type to load in model
            foreach (var notifType in Enum.GetValues(typeof(NotificationType)))
            {
                var currentSelectedChannels = new List<ProfileChannelModel>();

                //find and add selected channels for current notification type
                var currentMsgPrefsByType = currentMsgPrefs.Where(a => (a.NotificationType.Equals(notifType) && a.CustomerNumber == customerNumber));
                foreach (var currentMsgPref in currentMsgPrefsByType)
                {
                    currentSelectedChannels.Add(new ProfileChannelModel() { Channel = currentMsgPref.Channel, Description = EnumUtils<Channel>.GetDescription(currentMsgPref.Channel,"") });
                }
                msgPrefModelList.Add(new ProfileMessagingPreferenceDetailModel()
                {
                    NotificationType = (NotificationType)notifType,
                    Description = EnumUtils<NotificationType>.GetDescription((NotificationType)notifType, ""),
                    SelectedChannels = currentSelectedChannels
                });

            }
            return msgPrefModelList;
        }

        private List<ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid)
        {
            var currentMessagingPreferences = _msgServiceRepo.ReadMessagingPreferences(guid);
            var userCustomers = _customerRepo.GetCustomersForUser(guid);

            var returnedMsgPrefModel = new List<ProfileMessagingPreferenceModel>();
            //first load user preferences
            returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel()
            {
                Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, null)
            });
            //then load customer preferences
            foreach (var currentCustomer in userCustomers)
            {
                returnedMsgPrefModel.Add(new ProfileMessagingPreferenceModel()
                {
                    CustomerNumber = currentCustomer.CustomerNumber,
                    Preferences = BuildPreferenceModelForEachNotificationType(currentMessagingPreferences, currentCustomer.CustomerNumber)
                });
            }

            return returnedMsgPrefModel;
        }

        private string GetUserDsmRole(UserPrincipal user)
        {
            string dsmRole = _intAd.FirstUserGroup(user.UserPrincipalName, new List<string>() { Constants.ROLE_INTERNAL_DSM_FAM, Constants.ROLE_INTERNAL_DSM_FAQ, Constants.ROLE_INTERNAL_DSM_FDF,
                    Constants.ROLE_INTERNAL_DSM_FHS, Constants.ROLE_INTERNAL_DSM_FLR, Constants.ROLE_INTERNAL_DSM_FOK, Constants.ROLE_INTERNAL_DSM_FSA });
            return dsmRole;
        }

        private string GetUserDsrRole(UserPrincipal user)
        {
            string dsrRole = _intAd.FirstUserGroup(user.UserPrincipalName, new List<string>() { Constants.ROLE_INTERNAL_DSR_FAM, Constants.ROLE_INTERNAL_DSR_FAQ, Constants.ROLE_INTERNAL_DSR_FDF,
                    Constants.ROLE_INTERNAL_DSR_FHS, Constants.ROLE_INTERNAL_DSR_FLR, Constants.ROLE_INTERNAL_DSR_FOK, Constants.ROLE_INTERNAL_DSR_FSA });
            return dsrRole;
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
		public UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false)
		{
            // check for cached user profile first
            Core.Models.Profile.UserProfile profile = _cache.GetProfile(emailAddress);

            UserProfileReturn retVal = new UserProfileReturn();

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
                _cache.AddProfile(retVal.UserProfiles.FirstOrDefault());
            }
            return retVal;
        }

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
                roleName = "owner";
            } else {
                if (roleName == null && _extAd.IsInGroup(email, "owner")) {
                    roleName = "owner";
                } else if (roleName == null && _extAd.IsInGroup(email, "approver")) {
                    roleName = "approver";
                } else if (roleName == null && _extAd.IsInGroup(email, "buyer")) {
                    roleName = "buyer";
                } else if (roleName == null && _extAd.IsInGroup(email, "accounting")) {
                    roleName = "accounting";
                } else if (roleName == null && _extAd.IsInGroup(email, "guest")) {
                    roleName = "guest";
                }
            }

            return roleName;
        }

        //private string GetUserRole(UserPrincipal user) {
        //    PrincipalSearchResult<Principal> groups = user.GetGroups();

        //    foreach (GroupPrincipal group in groups) {
        //        group.Name
        //    }

        //    return null;
        //}

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
        public string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword) {
            string retVal = null;

            try {
                if (IsInternalAddress(emailAddress)) { throw new ApplicationException("Cannot change password for BEK user"); }

                UserProfile existingUser = GetUserProfile(emailAddress).UserProfiles[0];

                AssertPasswordLength(newPassword);
                AssertPasswordComplexity(newPassword);
                AssertPasswordVsAttributes(newPassword, existingUser.FirstName, existingUser.LastName);

                if (_extAd.UpdatePassword(emailAddress, originalPassword, newPassword)) {
                    retVal = "Password update successful";
                } else {
                    retVal = "Invalid password";
                }
            } catch (ApplicationException appEx) {
                retVal = appEx.Message;
            } catch (Exception ex) {
                retVal = string.Concat("Could not process request: ", ex.Message);
            }

            return retVal;
        }

        /// <summary>
        /// update the user profile in Commerce Server (not implemented)
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
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

            // remove the old user profile from cache and then update it with the new profile
            _cache.RemoveItem(existingUser.UserProfiles[0].EmailAddress);
            _cache.AddProfile(GetUserProfile(id).UserProfiles.FirstOrDefault());
        }
        #endregion
        
        public CustomerReturn GetCustomers(CustomerFilterModel customerFilters)
        {
            List<Customer> allCustomers = _customerRepo.GetCustomers();
            List<Customer> retCustomers = new List<Customer>();

            if (customerFilters != null)
            {
                if (customerFilters != null && !String.IsNullOrEmpty(customerFilters.AccountId)) {
                    retCustomers.AddRange(allCustomers.Where(x => x.AccountId == Guid.Parse(customerFilters.AccountId)));
                }
                if (customerFilters != null && !String.IsNullOrEmpty(customerFilters.UserId)) {
                    retCustomers.AddRange(GetUserProfile(customerFilters.UserId).UserProfiles[0].UserCustomers);
                }
                if (customerFilters != null && !String.IsNullOrEmpty(customerFilters.Wildcard)) {
                    retCustomers.AddRange(allCustomers.Where(x => x.CustomerName.ToLower().Contains(customerFilters.Wildcard.ToLower()) || x.CustomerNumber.ToLower().Contains(customerFilters.Wildcard.ToLower())));
                }
            }
            else
                retCustomers = allCustomers;
            
            // TODO: add logic to filter down for internal administration versus external owner

            return new CustomerReturn() { Customers = retCustomers.Distinct(new CustomerNumberComparer()).ToList() };
        }

        public AccountReturn GetAccounts(AccountFilterModel accountFilters)
        {
            List<Account> allAccounts = _accountRepo.GetAccounts();
            List<Account> retAccounts = new List<Account>();

            if (accountFilters != null) {
                if (accountFilters.UserId.HasValue) {
                    retAccounts.AddRange(_accountRepo.GetAccountsForUser(accountFilters.UserId.Value));

                }
                if (!String.IsNullOrEmpty(accountFilters.Wildcard)) {
                    retAccounts.AddRange(allAccounts.Where(x => x.Name.Contains(accountFilters.Wildcard)));
                }
            }
            else
                retAccounts = allAccounts;

            foreach (var acct in retAccounts)
            {
                acct.Customers = _customerRepo.GetCustomers().Where(x => x.AccountId == acct.Id).ToList();
            }
            // TODO: add logic to filter down for internal administration versus external owner

            return new AccountReturn() { Accounts = retAccounts.Distinct(new AccountComparer()).ToList() };
        }
        public AccountReturn GetAccount(Guid accountId)
        {
            List<Account> allAccounts = _accountRepo.GetAccounts();
            Account acct = allAccounts.Where(x => x.Id == accountId).FirstOrDefault();
            acct.Customers = _customerRepo.GetCustomers().Where(x => x.AccountId.Value == accountId).ToList();
            acct.Users = _csProfile.GetUsersForCustomerOrAccount(accountId);
            return new AccountReturn() { Accounts = new List<Account>() { acct } };
        }
        public void AddCustomerToAccount(Guid accountId, Guid customerId)
        {
            _accountRepo.AddCustomerToAccount(accountId, customerId);
        }


        public UserProfileReturn GetUsers(UserFilterModel userFilters)
        {
            if (userFilters != null)
            {
                if (userFilters.AccountId.HasValue)
                {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.AccountId.Value) };
                }
                else if (userFilters.CustomerId.HasValue)
                {
                    return new UserProfileReturn() { UserProfiles = _csProfile.GetUsersForCustomerOrAccount(userFilters.CustomerId.Value) };
                }
                else if (!String.IsNullOrEmpty(userFilters.Email))
                {
                    return GetUserProfile(userFilters.Email);
                }
            }

            throw new ApplicationException("No filter provided for users");
        }

        /// <summary>
        /// get the user profile by guid
        /// </summary>
        /// <remarks>
        /// jmmcmillan - 10/6/2014 - documented
        /// </remarks>
        public UserProfileReturn GetUserProfileByGuid(Guid UserId)
        {
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Id"] = "{fcbd9217-980f-4030-88c3-9a3e8d459fce}";//UserId.ToString();
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

            if (profileResponse.Count == 0)
            {
                /*
                 Throw profile not found exception??
                 */
            }
            else
            {
                retVal.UserProfiles.Add(FillUserProfile((Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0]));
            }

            return retVal;
        }


        public void AddUserToCustomer(Guid customerId, Guid userId)
        {
            _customerRepo.AddUserToCustomer(customerId, userId);
        }

        public void RemoveUserFromCustomer(Guid customerId, Guid userId)
        {
            _customerRepo.RemoveUserFromCustomer(customerId, userId);
        }

        public bool UpdateAccount(Guid accountId, string name, List<Customer> customers, List<UserProfile> users)
        {
            List<Customer> existingCustomers = _customerRepo.GetCustomers().Where(c => c.AccountId == accountId).ToList();
            List<UserProfile> existingUsers = _csProfile.GetUsersForCustomerOrAccount(accountId);

            IEnumerable<Guid> customersToAdd = customers.Select(c => c.CustomerId).Except(existingCustomers.Select(c => c.CustomerId));
            IEnumerable<Guid> customersToDelete = existingCustomers.Select(c => c.CustomerId).Except(customers.Select(c => c.CustomerId));
            IEnumerable<Guid> usersToAdd = users.Select(u => u.UserId).Except(existingUsers.Select(u => u.UserId));
            IEnumerable<Guid> usersToDelete = existingUsers.Select(u => u.UserId).Except(users.Select(u => u.UserId));

            foreach (Guid g in customersToAdd)
                _accountRepo.AddCustomerToAccount(accountId, g);
            foreach (Guid g in customersToDelete)
                _accountRepo.RemoveCustomerFromAccount(accountId, g);
            foreach (Guid g in usersToAdd)
                _accountRepo.AddUserToAccount(accountId, g);
            foreach (Guid g in usersToDelete)
                _accountRepo.RemoveUserFromAccount(accountId, g);

            // refresh cache; need to reload customers
            _customerRepo.ClearCustomerCache();
            return true;
        }
    }
}
