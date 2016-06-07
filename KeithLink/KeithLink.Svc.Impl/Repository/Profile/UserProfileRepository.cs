using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Impl.Helpers;

using CommerceServer.Foundation;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class UserProfileRepository : Core.Interface.Profile.IUserProfileRepository
    {
        #region attributes
        IEventLogRepository _logger;
		private readonly IAuditLogRepository _auditLog;
        #endregion

        #region ctor
        public UserProfileRepository(IEventLogRepository logger, IAuditLogRepository auditLog)
        {
            _logger = logger;
			_auditLog = auditLog;
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
        public void CreateUserProfile(string createdBy, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
            var createUser = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = firstName;
            createUser.Model.LastName = lastName;
            createUser.Model.Email = emailAddress;
            createUser.Model.Telephone = phoneNumber;

            Svc.Impl.Helpers.FoundationService.ExecuteRequest(createUser.ToRequest());

			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserCreated, createdBy, Newtonsoft.Json.JsonConvert.SerializeObject(createUser.Model));
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

        public List<Core.Models.Profile.UserProfile> GetUsersForCustomerOrAccount(Guid orgId)
        {
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["OrganizationId"] = orgId.ToCommerceServerFormat();

            CommerceServer.Foundation.CommerceResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());

            List<Core.Models.Profile.UserProfile> customerUsers = new List<Core.Models.Profile.UserProfile>();
            foreach (CommerceEntity ent in (res.OperationResponses[0] as CommerceQueryOperationResponse).CommerceEntities)
                customerUsers.Add(new Core.Models.Profile.UserProfile()
                {
                    UserId = Guid.Parse(ent.Id),
                    FirstName = (string)ent.Properties["FirstName"],
                    LastName = (string)ent.Properties["LastName"],
                    EmailAddress = (string)ent.Properties["Email"]
                });

            return customerUsers;
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
            profileQuery.Model.Properties.Add("LastLoginDate");
            profileQuery.Model.Properties.Add("LastActivityDate");
            profileQuery.Model.Properties.Add("FirstName");
            profileQuery.Model.Properties.Add("LastName");
            profileQuery.Model.Properties.Add("DefaultBranch");
            profileQuery.Model.Properties.Add("DefaultCustomer");
            profileQuery.Model.Properties.Add("Telephone");

            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            if (profileResponse.Count == 0) {
                return null;
            } else {
                return (Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0];
            }
        }

		public List<Core.Models.Generated.UserProfile> GetCSProfileForInternalUsers()
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("ProfileCustomSearch");
			queryOrg.SearchCriteria.WhereClause = "u_email_address like '%benekeith.com'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
			List<Core.Models.Generated.UserProfile> users = new List<Core.Models.Generated.UserProfile>();
			if (res.CommerceEntities.Count > 0)
			{
				foreach (CommerceEntity ent in res.CommerceEntities)
					users.Add((Core.Models.Generated.UserProfile)ent);			
			}

			return users;
		}

        /// <summary>
        /// retrieve the user's profile from commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>a commerce server user profile object</returns>
        /// <remarks>
        /// jwames - 10/14/14 - original code
        /// </remarks>
        public Core.Models.Generated.UserProfile GetCSProfile(Guid userId) {
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserProfile");
            profileQuery.SearchCriteria.Model.Properties["Id"] = userId.ToCommerceServerFormat();

			profileQuery.Model.Properties.Add("Id");
			profileQuery.Model.Properties.Add("Email");
            profileQuery.Model.Properties.Add("LastLoginDate");
            profileQuery.Model.Properties.Add("LastActivityDate");
            profileQuery.Model.Properties.Add("FirstName");
			profileQuery.Model.Properties.Add("LastName");
			profileQuery.Model.Properties.Add("DefaultBranch");
			profileQuery.Model.Properties.Add("DefaultCustomer");
			profileQuery.Model.Properties.Add("Telephone");

            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceServer.Foundation.CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceServer.Foundation.CommerceQueryOperationResponse;

            if (profileResponse.Count == 0) {
                return null;
            } else {
                return (Core.Models.Generated.UserProfile)profileResponse.CommerceEntities[0];
            }
        }

        public List<Core.Models.Profile.UserProfile> GetInternalUsers()
        {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("ProfileCustomSearch");
            queryOrg.SearchCriteria.WhereClause = "u_email_address like '%benekeith.com'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Core.Models.Profile.UserProfile> users = new List<Core.Models.Profile.UserProfile>();
            if (res.CommerceEntities.Count > 0)
            {
                foreach (CommerceEntity ent in res.CommerceEntities)
                {
                    users.Add(new Core.Models.Profile.UserProfile()
                    {
                        UserId = Guid.Parse(ent.Id),
                        FirstName = (string)ent.Properties["FirstName"],
                        LastName = (string)ent.Properties["LastName"],
                        EmailAddress = (string)ent.Properties["Email"]
                    });
                }
            }

            return users;
        }

        public List<Core.Models.Profile.UserProfile> GetExternalUsers()
        {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("ProfileCustomSearch");
            queryOrg.SearchCriteria.WhereClause = "u_email_address not like '%benekeith.com'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Core.Models.Profile.UserProfile> users = new List<Core.Models.Profile.UserProfile>();
            if (res.CommerceEntities.Count > 0)
            {
                foreach (CommerceEntity ent in res.CommerceEntities)
                {
                    users.Add(new Core.Models.Profile.UserProfile()
                    {
                        UserId = Guid.Parse(ent.Id),
                        FirstName = (string)ent.Properties["FirstName"],
                        LastName = (string)ent.Properties["LastName"],
                        EmailAddress = (string)ent.Properties["Email"]
                    });
                }
            }

            return users;
        }

        ///// <summary>
        ///// update the user profile in Commerce Server (not implemented)
        ///// </summary>
        ///// <remarks>
        ///// jwames - 8/18/2014 - documented
        ///// </remarks>
        public void UpdateUserProfile(string updatedBy, Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
            var updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.Email = emailAddress;
            updateQuery.Model.FirstName = firstName;
            updateQuery.Model.LastName = lastName;
            updateQuery.Model.Telephone = phoneNumber;
            updateQuery.Model.DefaultBranch = branchId;
            // TODO: add DefaultCustomer

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserUpdate, updatedBy, Newtonsoft.Json.JsonConvert.SerializeObject(updateQuery.Model));
        }


        public void UpdateUserProfileLastLogin(Guid id)
        {
            var updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.LastLoginDate = DateTime.Now;

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
            _auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserUpdate, null, Newtonsoft.Json.JsonConvert.SerializeObject(updateQuery.Model));
        }

        public void UpdateUserProfileLastAccess(Guid id)
        {
            var updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.LastActivityDate = DateTime.Now;

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
            _auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserUpdate, null, Newtonsoft.Json.JsonConvert.SerializeObject(updateQuery.Model));
        }
        #endregion
    }
}
