using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class UserProfileRepository : Core.Interface.Profile.IUserProfileRepository
    {
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
            ExternalUserDomainRepository extAd = new ExternalUserDomainRepository();

            if (extAd.GetUser(emailAddress) != null)
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
        private void AssertPasswordVsAttributes(string password, string customerName, string firstName, string lastName)
        {
            bool matched = false;

            if (string.Compare(password, customerName, true) == 0) { matched = true; }
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
            AssertPasswordVsAttributes(password, customerName, firstName, lastName);
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

                InternalUserDomainRepository internalAD = new InternalUserDomainRepository();
                return internalAD.AuthenticateUser(userName, password);
            }
            else
            {
                ExternalUserDomainRepository externalAD = new ExternalUserDomainRepository();
                return externalAD.AuthenticateUser(emailAddress, password);
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

                InternalUserDomainRepository internalAD = new InternalUserDomainRepository();
                return internalAD.AuthenticateUser(userName, password, out errorMessage);
            }
            else
            {
                ExternalUserDomainRepository externalAD = new ExternalUserDomainRepository();
                return externalAD.AuthenticateUser(emailAddress, password, out errorMessage);
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
        private UserProfile CombineProfileFromCSAndAD(Core.Models.Generated.UserProfile csProfile, string emailAddress)
        {
            System.DirectoryServices.AccountManagement.UserPrincipal adProfile = null;

            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                string userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));

                InternalUserDomainRepository internalAD = new InternalUserDomainRepository();
                adProfile = internalAD.GetUser(userName);
            }
            else
            {
                ExternalUserDomainRepository externalAD = new ExternalUserDomainRepository();
                adProfile = externalAD.GetUser(emailAddress);
            }

            return new UserProfile(){
                UserId = Guid.Parse(csProfile.Id),
                UserName = adProfile.UserPrincipalName,
                FirstName = csProfile.FirstName,
                LastName = csProfile.LastName,
                EmailAddress = csProfile.Email,
                PhoneNumber = adProfile.VoiceTelephoneNumber
            };
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

                Impl.Profile.ExternalUserDomainRepository extAD = new ExternalUserDomainRepository();
                userName = extAD.CreateUser(customerName, emailAddres, password, firstName, lastName, roleName);
            }

			var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = firstName;
            createUser.Model.LastName = lastName;
            createUser.Model.Email = emailAddres;

            createUser.CreateOptions.ReturnModel.Properties.Add("Id");
            
            CommerceServer.Foundation.CommerceRequestContext requestContext = new CommerceServer.Foundation.CommerceRequestContext();
            
            // indicate the default channel
            requestContext.Channel = string.Empty;
            requestContext.RequestId = System.Guid.NewGuid().ToString("B");
            requestContext.UserLocale = "en-US";
            requestContext.UserUILocale = "en-US";
            
            // Execute the operation and get the results back
            CommerceServer.Foundation.OperationServiceAgent serviceAgent = new CommerceServer.Foundation.OperationServiceAgent();
            CommerceServer.Foundation.CommerceResponse response = serviceAgent.ProcessRequest(requestContext, createUser.ToRequest());

            CommerceServer.Foundation.CommerceCreateOperationResponse createResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceCreateOperationResponse;

            return (GetUserProfile(emailAddres));
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

        /// <summary>
        /// get the user profile
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserProfileReturn GetUserProfile(string userName)
        {
			var profileQuery = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Email"] = userName;
            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");


            // create the request
            CommerceServer.Foundation.CommerceRequestContext requestContext = new CommerceServer.Foundation.CommerceRequestContext();
            requestContext.Channel = string.Empty;
            requestContext.RequestId = System.Guid.NewGuid().ToString("B");
            requestContext.UserLocale = "en-US";
            requestContext.UserUILocale = "en-US";

            // Execute the operation and get the results back
            CommerceServer.Foundation.OperationServiceAgent serviceAgent = new CommerceServer.Foundation.OperationServiceAgent();
            CommerceServer.Foundation.CommerceResponse response = serviceAgent.ProcessRequest(requestContext, profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;


            UserProfileReturn retVal = new UserProfileReturn();
			retVal.UserProfiles.Add(CombineProfileFromCSAndAD((Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0], userName));

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

        /// <summary>
        /// update the user profile in Commerce Server (not implemented)
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public void UpdateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
