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
    public class AccountRepository : Core.Interface.Profile.IAccountRepository
    {
        #region attributes
        IEventLogRepository _logger;
        IUserProfileCacheRepository _userProfileCacheRepository;
        #endregion

        #region ctor
        public AccountRepository(IEventLogRepository logger, IUserProfileCacheRepository userProfileCacheRepository)
        {
            _logger = logger;
            _userProfileCacheRepository = userProfileCacheRepository;
        }
        #endregion

        #region methods
        /// <summary>
        /// create an account in Commerce Server
        /// </summary>
        /// <param name="name">Account Name</param>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public void CreateAccount(string name) {
            KeithLink.Svc.Core.Models.Generated.SiteTerm orgTypes = GetOrganizationTypes();
            string accountOrgTypeId = orgTypes.Elements.Where(o => o.DisplayName == "Account").FirstOrDefault().Id;

            var createOrg = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            createOrg.Model.GeneralInfoname = name;
            //createOrg.Model.GeneralInfonatlOrReglAccountNumber = nationalOrRegionalAccountNumber;
            // createOrg.Model.GeneralInfonationalAccountId = ""; // TODO: Data does not exist in currend data feeds
            createOrg.Model.GeneralInfoorganizationType = accountOrgTypeId;

            Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest());
        }

        public void AddUserToAccount(string accountId, string userId, string role)
        {
        }

        public void AddCustomerToAccount(string accountId, string customerId)
        {
        }

        public void AddUserToCustomer(string customerId, string userId, string role)
        {
        }

        public void RemoveUserFromAccount(string accountId, string userId, string role)
        {
        }

        public void RemoveCustomerFromAccount(string accountId, string customerId)
        {
        }

        public void RemoveUserFromCustomer(string accountId, string userId, string role)
        {
        }

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
        public void CreateNationalId(string name, string adminEmail, string nationalId)
        {
            KeithLink.Svc.Core.Models.Generated.SiteTerm orgTypes = GetOrganizationTypes();

            var createOrg = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            createOrg.Model.GeneralInfoname = name;
            createOrg.Model.GeneralInfonatlOrReglAccountNumber = "";
            createOrg.Model.GeneralInfonationalAccountId = nationalId;
            createOrg.Model.GeneralInfoorganizationType = "National ID"; // TODO: Read from site term, validate and convert to id

            Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest());
        }

        protected KeithLink.Svc.Core.Models.Generated.SiteTerm GetOrganizationTypes()
        {
            var siteTermQuery = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.SiteTerm>("SiteTerm");
            siteTermQuery.SearchCriteria.Model.Properties["Id"] = "OrganizationType";
            siteTermQuery.RelatedOperations.Add(
                new CommerceQueryRelatedItem<KeithLink.Svc.Core.Models.Generated.SiteTermElement>
                    (KeithLink.Svc.Core.Models.Generated.SiteTerm.RelationshipName.Elements));
            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(siteTermQuery.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            return new KeithLink.Svc.Core.Models.Generated.SiteTerm(res.CommerceEntities[0]);
        }

        #endregion
    }
}
