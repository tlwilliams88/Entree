using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class UserProfileRepository : Core.Profile.IUserProfileRepository
    {
        #region methods
        private void AssertCustomerNameLength(string customerName)
        {
            if (customerName.Length == 0) throw new ApplicationException("Customer name is blank");
        }

        private void AssertCustomerNameValidCharacters(string customerName)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ApplicationException("Invalid characters in customer name"); }
        }

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

        private void AssertEmailAddressLength(string emailAddress)
        {
            if (emailAddress.Length == 0) throw new ApplicationException("Email address is blank");
        }

        private void AssertEmailAddressUnique(string emailAddress){
            ExternalUserDomainRepository extAd = new ExternalUserDomainRepository();

            if (extAd.GetUser(emailAddress) != null)
            {
                throw new ApplicationException("Email address is already in use");
            }
        }

        private void AssertFirstNameLength(string firstName)
        {
            if (firstName.Length == 0) throw new ApplicationException("First name is blank");
        }

        private void AssertLastNameLength(string lastName)
        {
            if (lastName.Length == 0) throw new ApplicationException("Last name is blank");
        }

        private void AssertPasswordComplexity(string password)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(password, Core.Constants.REGEX_PASSWORD_PATTERN) == false) { 
                throw new ApplicationException("Password must contain 1 upper and 1 lower case letter and 1 number"); 
            }
        }

        private void AssertPasswordLength(string password)
        {
            if (password.Length < 7) throw new ApplicationException("Minimum password length is 7 characters");
        }

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

        private void AssertRoleNameLength(string roleName)
        {
            if (roleName.Length == 0) { throw new ApplicationException("Role name is blank"); }
        }

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

        public bool AuthenticateUser(string emailAddress, string password, out Core.Profile.UserProfileReturn userProfile)
        {
            bool success = AuthenticateUser(emailAddress, password);

            if (success) { 
                userProfile = GetUserProfile(emailAddress); 
            } else {
                userProfile = new Core.Profile.UserProfileReturn();
            }

            return success;
        }

        private Core.Profile.UserProfile CombineProfileFromCSAndAD(Models.Generated.UserProfile csProfile, string emailAddress)
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

            return new Core.Profile.UserProfile(){
                UserId = csProfile.Id,
                UserName = adProfile.UserPrincipalName,
                FirstName = csProfile.FirstName,
                LastName = csProfile.LastName,
                EmailAddress = csProfile.Email,
                PhoneNumber = adProfile.VoiceTelephoneNumber
            };
        }

        public Core.Profile.UserProfileReturn CreateUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName)
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

            var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Impl.Models.Generated.UserProfile>("UserProfile");

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

        public void DeleteUserProfile(string userName)
        {
            throw new NotImplementedException();
        }

        public Core.Profile.UserProfileReturn GetUserProfile(string userName)
        {
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Impl.Models.Generated.UserProfile>("UserProfile");
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


            Core.Profile.UserProfileReturn retVal = new Core.Profile.UserProfileReturn();
            retVal.UserProfiles.Add(CombineProfileFromCSAndAD((Models.Generated.UserProfile)profileResponse.CommerceEntities[0], userName));
            
            return retVal;
        }

        public Core.Profile.UserProfileReturn GetUserProfilesByCustomerName(string customerName)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
