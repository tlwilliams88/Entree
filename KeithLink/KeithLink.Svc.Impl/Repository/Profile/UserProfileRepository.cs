using System;
using System.Collections.Generic;

using CommerceServer.Foundation;
using Newtonsoft.Json;

using KeithLink.Common.Core.Enumerations;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Seams;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class UserProfileRepository : IUserProfileRepository {
        #region ctor
        public UserProfileRepository(IEventLogRepository logger, IAuditLogRepository auditLog) {
            _logger = logger;
            _auditLog = auditLog;
        }
        #endregion

        #region attributes
        private IEventLogRepository _logger;
        private readonly IAuditLogRepository _auditLog;
        #endregion

        #region methods
        /// <summary>
        ///     create a profile for the user in commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="firstName">user's given name</param>
        /// <param name="lastName">user's surname</param>
        /// <param name="phoneNumber">user's telephone number</param>
        /// <remarks>
        ///     jwames - 10/3/2014 - documented
        /// </remarks>
        public void CreateUserProfile(string createdBy, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId) {
            CommerceCreate<Core.Models.Generated.UserProfile> createUser = new CommerceCreate<Core.Models.Generated.UserProfile>("UserProfile");

            createUser.Model.FirstName = firstName;
            createUser.Model.LastName = lastName;
            createUser.Model.Email = emailAddress;
            createUser.Model.Telephone = phoneNumber;
            createUser.Model.DefaultBranch = branchId;

            FoundationService.ExecuteRequest(createUser.ToRequest());

            _auditLog.WriteToAuditLog(AuditType.UserCreated, createdBy, JsonConvert.SerializeObject(createUser.Model));
        }

        /// <summary>
        ///     delete the user from Commerce Server (not implemented)
        /// </summary>
        /// <remarks>
        ///     jwames - 8/18/2014 - documented
        /// </remarks>
        public void DeleteUserProfile(string userName) {
            throw new NotImplementedException();
        }

        public List<UserProfile> GetUsersForCustomerOrAccount(Guid orgId) {
            CommerceQuery<CommerceEntity> profileQuery = new CommerceQuery<CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["OrganizationId"] = orgId.ToCommerceServerFormat();

            CommerceResponse res = Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());

            List<UserProfile> customerUsers = new List<UserProfile>();

            Dictionary<Guid, bool> existingUsers = new Dictionary<Guid, bool>();

            foreach (CommerceEntity ent in (res.OperationResponses[0] as CommerceQueryOperationResponse).CommerceEntities) {
                Guid userid = Guid.Parse(ent.Id);

                if (!existingUsers.ContainsKey(userid)) {
                    existingUsers.Add(userid, true);

                    customerUsers.Add(new UserProfile {
                        UserId = userid,
                        FirstName = (string) ent.Properties["FirstName"],
                        LastName = (string) ent.Properties["LastName"],
                        EmailAddress = (string) ent.Properties["Email"]
                    });
                }
            }

            return customerUsers;
        }

        /// <summary>
        ///     retrieve the user's profile from commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>a commerce server user profile object</returns>
        /// <remarks>
        ///     jwames - 10/3/2014 - documented
        /// </remarks>
        public Core.Models.Generated.UserProfile GetCSProfile(string emailAddress) {
            CommerceQuery<CommerceEntity> profileQuery = new CommerceQuery<CommerceEntity>("UserProfile");
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

            CommerceResponse response = Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

            if (profileResponse.Count == 0) {
                return null;
            }
            return (Core.Models.Generated.UserProfile) profileResponse.CommerceEntities[0];
        }

        public List<Core.Models.Generated.UserProfile> GetCSProfileForInternalUsers() {
            CommerceQuery<CommerceEntity> queryOrg = new CommerceQuery<CommerceEntity>("ProfileCustomSearch");
            queryOrg.SearchCriteria.WhereClause = "u_email_address like '%benekeith.com'"; // org type of customer

            CommerceQueryOperationResponse res = Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())
                                                        .OperationResponses[0] as CommerceQueryOperationResponse;
            List<Core.Models.Generated.UserProfile> users = new List<Core.Models.Generated.UserProfile>();
            if (res.CommerceEntities.Count > 0) {
                foreach (CommerceEntity ent in res.CommerceEntities) {
                    users.Add((Core.Models.Generated.UserProfile) ent);
                }
            }

            return users;
        }

        /// <summary>
        ///     retrieve the user's profile from commerce server
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>a commerce server user profile object</returns>
        /// <remarks>
        ///     jwames - 10/14/14 - original code
        /// </remarks>
        public Core.Models.Generated.UserProfile GetCSProfile(Guid userId) {
            CommerceQuery<CommerceEntity> profileQuery = new CommerceQuery<CommerceEntity>("UserProfile");
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
            CommerceResponse response = Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());
            CommerceQueryOperationResponse profileResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

            if (profileResponse.Count == 0) {
                return null;
            }
            return (Core.Models.Generated.UserProfile) profileResponse.CommerceEntities[0];
        }

        public List<UserProfile> GetInternalUsers() {
            CommerceQuery<CommerceEntity> queryOrg = new CommerceQuery<CommerceEntity>("ProfileCustomSearch");
            queryOrg.SearchCriteria.WhereClause = "u_email_address like '%benekeith.com'"; // org type of customer

            CommerceQueryOperationResponse res = Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())
                                                        .OperationResponses[0] as CommerceQueryOperationResponse;
            List<UserProfile> users = new List<UserProfile>();
            if (res.CommerceEntities.Count > 0) {
                foreach (CommerceEntity ent in res.CommerceEntities) {
                    users.Add(new UserProfile {
                        UserId = Guid.Parse(ent.Id),
                        FirstName = (string) ent.Properties["FirstName"],
                        LastName = (string) ent.Properties["LastName"],
                        EmailAddress = (string) ent.Properties["Email"]
                    });
                }
            }

            return users;
        }

        public List<UserProfile> GetExternalUsers() {
            CommerceQuery<CommerceEntity> queryOrg = new CommerceQuery<CommerceEntity>("ProfileCustomSearch");
            queryOrg.SearchCriteria.WhereClause = "u_email_address not like '%benekeith.com'"; // org type of customer

            CommerceQueryOperationResponse res = Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())
                                                        .OperationResponses[0] as CommerceQueryOperationResponse;
            List<UserProfile> users = new List<UserProfile>();
            if (res.CommerceEntities.Count > 0) {
                foreach (CommerceEntity ent in res.CommerceEntities) {
                    users.Add(new UserProfile {
                        UserId = Guid.Parse(ent.Id),
                        FirstName = (string) ent.Properties["FirstName"],
                        LastName = (string) ent.Properties["LastName"],
                        EmailAddress = (string) ent.Properties["Email"]
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
            CommerceUpdate<Core.Models.Generated.UserProfile> updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.Email = emailAddress;
            updateQuery.Model.FirstName = firstName;
            updateQuery.Model.LastName = lastName;
            updateQuery.Model.Telephone = phoneNumber;
            updateQuery.Model.DefaultBranch = branchId;
            // TODO: add DefaultCustomer

            CommerceResponse response = Helpers.FoundationService.ExecuteRequest(updateQuery.ToRequest());
            _auditLog.WriteToAuditLog(AuditType.UserUpdate, updatedBy, JsonConvert.SerializeObject(updateQuery.Model));
        }

        public void UpdateUserProfileLastLogin(Guid id) {
            CommerceUpdate<Core.Models.Generated.UserProfile> updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.LastLoginDate = DateTime.Now;

            CommerceResponse response = Helpers.FoundationService.ExecuteRequest(updateQuery.ToRequest());
            _auditLog.WriteToAuditLog(AuditType.UserUpdate, null, JsonConvert.SerializeObject(updateQuery.Model));
        }

        public void UpdateUserProfileLastAccess(Guid id) {
            CommerceUpdate<Core.Models.Generated.UserProfile> updateQuery = new CommerceUpdate<Core.Models.Generated.UserProfile>("UserProfile");
            updateQuery.SearchCriteria.Model.Properties["Id"] = id.ToCommerceServerFormat();

            updateQuery.Model.LastActivityDate = DateTime.Now;

            CommerceResponse response = Helpers.FoundationService.ExecuteRequest(updateQuery.ToRequest());
            //_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserUpdate, null, Newtonsoft.Json.JsonConvert.SerializeObject(updateQuery.Model));
        }
        #endregion
    }
}