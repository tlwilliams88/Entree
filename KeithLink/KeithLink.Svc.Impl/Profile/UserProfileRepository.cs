using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class UserProfileRepository : Core.Profile.IUserProfileRepository
    {
        #region methods

        public void CreateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber)
        {
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

            throw new NotImplementedException();
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
