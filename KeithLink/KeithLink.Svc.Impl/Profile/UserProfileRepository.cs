using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class UserProfileRepository : Core.Profile.IUserProfileRepository
    {
        #region methods
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

        public void CreateUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName)
        {
            string userName = null;

            if (System.Text.RegularExpressions.Regex.IsMatch(emailAddres, Core.Constants.REGEX_BENEKEITHEMAILADDRESS))
            {
                // cannot create a user on the internal server
            }
            else
            {
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
