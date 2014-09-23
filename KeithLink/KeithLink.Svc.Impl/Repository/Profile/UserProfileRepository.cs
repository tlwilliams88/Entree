using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class UserProfileRepository : Core.Interface.Profile.IUserProfileRepository
    {
        #region attributes
        IEventLogRepository _logger;
        InternalUserDomainRepository _internalAD;
        ExternalUserDomainRepository _externalAD;
        IUserProfileCacheRepository _userProfileCacheRepository;
        #endregion

        #region ctor
        public UserProfileRepository(IEventLogRepository logger, ExternalUserDomainRepository externalAD, InternalUserDomainRepository internalAD, IUserProfileCacheRepository userProfileCacheRepository)
        {
            _logger = logger;
            _internalAD = internalAD;
            _externalAD = externalAD;
            _userProfileCacheRepository = userProfileCacheRepository;
        }
        #endregion

        #region methods
        /// <summary>
        /// check that the customer name is longer the 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertCustomerNameLength(string customerName)
        {
            if (customerName.Length == 0) throw new ApplicationException("Customer name is blank");
        }

        /// <summary>
        /// make sure that there are not any special characters in the customer name
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertCustomerNameValidCharacters(string customerName)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ApplicationException("Invalid characters in customer name"); }
        }

        /// <summary>
        /// test to make sure that it is a correctly formatted email address
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddress(string emailAddress)
        {
            try
            {
                System.Net.Mail.MailAddress testAddress = new System.Net.Mail.MailAddress(emailAddress);
            }
            catch
            {
                throw new ApplicationException("Invalid email address");
            }
        }

        /// <summary>
        /// make sure that the email address is longer the 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddressLength(string emailAddress)
        {
            if (emailAddress.Length == 0) throw new ApplicationException("Email address is blank");
        }

        /// <summary>
        /// make sure that the email address does not already exist
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertEmailAddressUnique(string emailAddress){
            if (_externalAD.GetUser(emailAddress) != null)
            {
                throw new ApplicationException("Email address is already in use");
            }
        }

        /// <summary>
        /// make sure that the first name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertFirstNameLength(string firstName)
        {
            if (firstName.Length == 0) throw new ApplicationException("First name is blank");
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
            AssertPasswordComplexity(password);
            AssertPasswordLength(password);
        }

        /// <summary>
        /// make sure that the last name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertLastNameLength(string lastName)
        {
            if (lastName.Length == 0) throw new ApplicationException("Last name is blank");
        }

        /// <summary>
        /// make sure that the password meets our complexity rules of 1 upper case letter, 1 lower case letter, and 1 number
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertPasswordComplexity(string password)
        {
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
        private void AssertPasswordLength(string password)
        {
            if (password.Length < 7) throw new ApplicationException("Minimum password length is 7 characters");
        }

        /// <summary>
        /// make sure that the password does not contain any of the values stored in the attributes such as first or last name
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertPasswordVsAttributes(string password, string firstName, string lastName)
        {
            bool matched = false;

            //if (string.Compare(password, customerName, true) == 0) { matched = true; }
            if (string.Compare(password, firstName, true) == 0) { matched = true; }
            if (string.Compare(password, lastName, true) == 0) { matched = true; }

            if (matched)
            {
                throw new ApplicationException("Invalid password");
            }
        }

        /// <summary>
        /// make sure that the role name is a valid role
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertRoleName(string roleName)
        {
            bool found = false;

            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_ACCOUNTING, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_OWNER, true) == 0) { found = true; }
            if (string.Compare(roleName, Core.Constants.ROLE_EXTERNAL_PURCHASING, true) == 0) { found = true; }

            if (found == false)
            {
                throw new ApplicationException("Role name is unknown");
            }
        }

        /// <summary>
        /// make sure that the role name is longer than 0 characters
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertRoleNameLength(string roleName)
        {
            if (roleName.Length == 0) { throw new ApplicationException("Role name is blank"); }
        }

        /// <summary>
        /// validate all of the attributes of the user's profile
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private void AssertUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName)
        {
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

        /// <summary>
        /// test the user against internal or external AD based on the email address' domain name. Will throw an exception if authentication fails
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public bool AuthenticateUser(string emailAddress, string password)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                string userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));

                return _internalAD.AuthenticateUser(userName, password);
            }
            else
            {
                return _externalAD.AuthenticateUser(emailAddress, password);
            }
        }

        /// <summary>
        /// authenticate a user against internal or external AD based on the email address' domain. Authentication failure does not throw an exception
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="password">the user's password</param>
        /// <param name="errorMessage">authentication failure messages</param>
        /// <returns>true/false</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public bool AuthenticateUser(string emailAddress, string password, out string errorMessage)
        {
            errorMessage = null;

            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                string userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));

                return _internalAD.AuthenticateUser(userName, password, out errorMessage);
            }
            else
            {
                return _externalAD.AuthenticateUser(emailAddress, password, out errorMessage);
            }
        }

        /// <summary>
        /// combines the attributes from AD and Commerce Server into our UserProfile class
        /// </summary>
        /// <param name="csProfile">commerce server user profile</param>
        /// <param name="emailAddress">the user's email address to lookup in AD</param>
        /// <returns>UserProfile object with attributes filled from both locations</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        private UserProfile CombineProfileFromCSAndAD(Core.Models.Generated.UserProfile csProfile)
        {
            System.DirectoryServices.AccountManagement.UserPrincipal adProfile = null;

            if (System.Text.RegularExpressions.Regex.IsMatch(csProfile.Email, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                adProfile = _internalAD.GetUserByEmailAddress(csProfile.Email);
            }
            else
            {
                adProfile = _externalAD.GetUser(csProfile.Email);
            }

            return new UserProfile(){
                UserId = Guid.Parse(csProfile.Id),
                //UserName = adProfile.UserPrincipalName,
                FirstName = csProfile.FirstName,
                LastName = csProfile.LastName,
                EmailAddress = csProfile.Email,
                PhoneNumber = csProfile.PhoneNumber,
                CustomerNumber = csProfile.SelectedCustomer,
                BranchId = csProfile.SelectedBranch
            };
        }

        /// <summary>
        /// create a Commerce Server User Profile for a BEK user
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <remarks>
        /// jwames - 8/29/2014 - original code
        /// </remarks>
        public void CreateBekUserProfile(string emailAddress)
        {
            var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            System.DirectoryServices.AccountManagement.UserPrincipal bekUser = _internalAD.GetUserByEmailAddress(emailAddress);
            string fName = bekUser.DisplayName.Split(' ')[0];

            createUser.Model.FirstName = fName;
            createUser.Model.LastName = bekUser.Surname;
            createUser.Model.Email = emailAddress;

            createUser.CreateOptions.ReturnModel.Properties.Add("Id");

            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createUser.ToRequest());

            CommerceServer.Foundation.CommerceCreateOperationResponse createResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceCreateOperationResponse;
        }

        /// <summary>
        /// create the user in AD if external address, and always create a Commerce Server profile
        /// </summary>
        /// <returns>a completed user profile</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserProfileReturn CreateUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName)
        {
            AssertUserProfile(customerName, emailAddres, password, firstName, lastName, phoneNumber, roleName);

            string userName = null;

            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddres, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                // cannot create a user on the internal server
            }
            else
            {
                AssertEmailAddressUnique(emailAddres);

                userName = _externalAD.CreateUser(customerName, emailAddres, password, firstName, lastName, roleName);
            }

			var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = firstName;
            createUser.Model.LastName = lastName;
            createUser.Model.Email = emailAddres;

            createUser.CreateOptions.ReturnModel.Properties.Add("Id");
            
            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createUser.ToRequest());

            CommerceServer.Foundation.CommerceCreateOperationResponse createResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceCreateOperationResponse;

            return (GetUserProfile(emailAddres));
        }

        /// <summary>
        /// creates a guest user account in AD and the user profile in CS
        /// </summary>
        /// <returns>UserProfileReturn</returns>
        /// <remarks>
        /// jwames - 9/16/2014 - original code
        /// </remarks>
        public UserProfileReturn CreateGuestProfile(string emailAddress, string password, string branchId) {
            AssertGuestProfile(emailAddress, password);
        
            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS)) {
                throw new ApplicationException("Cannot create a guest profile for a BEK address");
            } else {
                AssertEmailAddressUnique(emailAddress);

                _externalAD.CreateUser(Core.Constants.AD_GUEST_CONTAINER, 
                                       emailAddress, 
                                       password, 
                                       Core.Constants.AD_GUEST_FIRSTNAME, 
                                       Core.Constants.AD_GUEST_LASTNAME, 
                                       Core.Constants.ROLE_EXTERNAL_GUEST);
            }


            var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = Core.Constants.AD_GUEST_FIRSTNAME;
            createUser.Model.LastName = Core.Constants.AD_GUEST_LASTNAME;
            createUser.Model.Email = emailAddress;
            createUser.Model.SelectedBranch = branchId;

            createUser.CreateOptions.ReturnModel.Properties.Add("Id");

            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createUser.ToRequest());
            CommerceServer.Foundation.CommerceCreateOperationResponse createResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceCreateOperationResponse;

            return (GetUserProfile(emailAddress));
        }

        /// <summary>
        /// delete the user from Commerce Server (not implemented)
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public void DeleteUserProfile(string userName)
        {
            throw new NotImplementedException();
        }

        public UserProfileReturn GetUserProfile(Guid userId) {
            //Core.Models.Profile.UserProfile upFromCache = null;
            //upFromCache = _userProfileCacheRepository.GetProfile(emailAddress);
            //if (_userProfileCacheRepository.GetProfile(emailAddress) != null) {
            //    return new UserProfileReturn() { UserProfiles = new List<UserProfile>() { upFromCache } };
            //}

            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Id"] = userId.ToString("B");

            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("SelectedBranch");
            profileQuery.Model.Properties.Add("SelectedCustomer");
            profileQuery.Model.Properties.Add("PhoneNumber");

            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            UserProfileReturn retVal = new UserProfileReturn();

            if (profileResponse.Count == 0) {
            } else {
                retVal.UserProfiles.Add(CombineProfileFromCSAndAD((Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0]));
            }

            if (retVal != null) {
                _userProfileCacheRepository.AddProfile(retVal.UserProfiles.FirstOrDefault());
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
        public UserProfileReturn GetUserProfile(string emailAddress)
        {
            Core.Models.Profile.UserProfile upFromCache = null;
            upFromCache = _userProfileCacheRepository.GetProfile(emailAddress);
            if (_userProfileCacheRepository.GetProfile(emailAddress) != null)
            {
                return new UserProfileReturn() { UserProfiles = new List<UserProfile>() { upFromCache } };
            }

            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Email"] = emailAddress;
            profileQuery.SearchCriteria.Model.DateModified = DateTime.Now;

            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("SelectedBranch");
            profileQuery.Model.Properties.Add("SelectedCustomer");
            profileQuery.Model.Properties.Add("PhoneNumber");

            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            UserProfileReturn retVal = new UserProfileReturn();

            if (profileResponse.Count == 0)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
                {
                    CreateBekUserProfile(emailAddress);

                    return GetUserProfile(emailAddress);
                }
            }
            else
            {
                retVal.UserProfiles.Add(CombineProfileFromCSAndAD((Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0]));
            }

            if (retVal != null)
            {
                _userProfileCacheRepository.AddProfile(retVal.UserProfiles.FirstOrDefault());
            }
            return retVal;
        }

        /// <summary>
        /// get all of the users for the customer name (not implemented)
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserProfileReturn GetUserProfilesByCustomerName(string customerName)
        {
            throw new NotImplementedException();
        }

        public string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword) {
            string retVal = null;

            try {
                UserProfile existingUser = GetUserProfile(emailAddress).UserProfiles[0];

                AssertPasswordLength(newPassword);
                AssertPasswordComplexity(newPassword);
                AssertPasswordVsAttributes(newPassword, existingUser.FirstName, existingUser.LastName);

                if (_externalAD.UpdatePassword(emailAddress, originalPassword, newPassword)) {
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
        public void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId)
        {
            AssertEmailAddressLength(emailAddress);
            AssertEmailAddress(emailAddress);
            AssertFirstNameLength(firstName);
            AssertLastNameLength(lastName);


            UserProfileReturn existingUser = GetUserProfile(id);

            if (string.Compare(existingUser.UserProfiles[0].EmailAddress, emailAddress, true) != 0) { AssertEmailAddressUnique(emailAddress); }

            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS) || 
                System.Text.RegularExpressions.Regex.IsMatch(existingUser.UserProfiles[0].EmailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS)) {
                throw new ApplicationException("Cannot update profile information for BEK user.");
            }

            var updateQuery = new CommerceUpdate <CommerceEntity>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToString("B");

            updateQuery.Model.Properties["Email"] = emailAddress;
            updateQuery.Model.Properties["FirstName"] = firstName;
            updateQuery.Model.Properties["LastName"] = lastName;
            updateQuery.Model.Properties["PhoneNumber"] = phoneNumber;
            updateQuery.Model.Properties["SelectedBranch"] = branchId;

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());

            _externalAD.UpdateUserAttributes(existingUser.UserProfiles[0].EmailAddress, emailAddress, firstName, lastName);

            // remove the old user profile from cache and then update it with the new profile
            _userProfileCacheRepository.RemoveItem(existingUser.UserProfiles[0].EmailAddress);
            _userProfileCacheRepository.AddProfile(GetUserProfile(id).UserProfiles.FirstOrDefault());
        }
        #endregion
    }
}
