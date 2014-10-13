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
        public Guid CreateAccount(string name) {
            KeithLink.Svc.Core.Models.Generated.SiteTerm orgTypes = GetOrganizationTypes();
            string accountOrgTypeId = orgTypes.Elements.Where(o => o.DisplayName == "Account").FirstOrDefault().Id;

            var createOrg = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            createOrg.Model.Name = name;
            //createOrg.Model.NationalOrRegionalAccountNumber = nationalOrRegionalAccountNumber;
            // createOrg.Model.NationalAccountId = ""; // TODO: Data does not exist in currend data feeds
            createOrg.Model.OrganizationType = accountOrgTypeId;
            createOrg.CreateOptions.ReturnModel = new Core.Models.Generated.Organization();

            CommerceCreateOperationResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest()).OperationResponses[0] as CommerceCreateOperationResponse;

            return new Guid(res.CommerceEntity.Id);
        }

        public List<Account> GetAccounts()
        {
            var createOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            createOrg.SearchCriteria.Model.OrganizationType = "1"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var accounts = new System.Collections.Concurrent.BlockingCollection<Account>();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    KeithLink.Svc.Core.Models.Generated.Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    accounts.Add(new Account()
                    {
                        Id = org.Id,
                        Name = org.Name,
                    });
                });

            return  accounts.ToList();
        }

        public void AddCustomerToAccount(Guid accountId, Guid customerId)
        {
            var updateQuery = new CommerceUpdate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            updateQuery.SearchCriteria.Model.Properties["Id"] = customerId.ToCommerceServerFormat();

            updateQuery.Model.ParentOrganizationId = accountId.ToCommerceServerFormat();

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
        }

        public void AddUserToAccount(Guid accountId, Guid userId, string role)
        {
        }
        
        public void RemoveUserFromAccount(Guid accountId, Guid userId)
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

            createOrg.Model.Name = name;
            createOrg.Model.NationalOrRegionalAccountNumber = "";
            createOrg.Model.NationalAccountId = nationalId;
            createOrg.Model.OrganizationType = "National ID"; // TODO: Read from site term, validate and convert to id

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
