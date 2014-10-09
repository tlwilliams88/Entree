using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class UserProfileRepository : Core.Interface.Profile.IUserProfileRepository
    {
        #region attributes
        IEventLogRepository _logger;
        IUserProfileCacheRepository _userProfileCacheRepository;
        #endregion

        #region ctor
        public UserProfileRepository(IEventLogRepository logger, IUserProfileCacheRepository userProfileCacheRepository)
        {
            _logger = logger;
            _userProfileCacheRepository = userProfileCacheRepository;
        }
        #endregion

        #region methods
        /// <summary>
        /// create a profile for the user in commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="firstName">user's given name</param>
        /// <param name="lastName">user's surname</param>
        /// <param name="phoneNumber">user's telephone number</param>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public void CreateUserProfile(string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
            var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = firstName;
            createUser.Model.LastName = lastName;
            createUser.Model.Email = emailAddress;
            createUser.Model.GeneralInfotelNumber = phoneNumber;

            Svc.Impl.Helpers.FoundationService.ExecuteRequest(createUser.ToRequest());
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
        /// retrieve the user's profile from commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>a commerce server user profile object</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public Core.Models.Generated.UserProfile GetCSProfile(string emailAddress) {
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Email"] = emailAddress;
            profileQuery.SearchCriteria.Model.DateModified = DateTime.Now;

            profileQuery.Model.Properties.Add("Id");
            profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("GeneralInfo.default_branch");
            profileQuery.Model.Properties.Add("GeneralInfo.default_customer");
            profileQuery.Model.Properties.Add("GeneralInfo.tel_number");

            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            if (profileResponse.Count == 0) {
                return null;
            } else {
                return (Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0];
            }
        }

        ///// <summary>
        ///// update the user profile in Commerce Server (not implemented)
        ///// </summary>
        ///// <remarks>
        ///// jwames - 8/18/2014 - documented
        ///// </remarks>
        public void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
            var updateQuery = new CommerceUpdate<CommerceEntity>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.Properties["Email"] = emailAddress;
            updateQuery.Model.Properties["FirstName"] = firstName;
            updateQuery.Model.Properties["LastName"] = lastName;
            updateQuery.Model.Properties["GeneralInfo.tel_number"] = phoneNumber;
            updateQuery.Model.Properties["GeneralInfo.default_branch"] = branchId;
            // TODO: add DefaultCustomer

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
        }
        #endregion
    }
}
