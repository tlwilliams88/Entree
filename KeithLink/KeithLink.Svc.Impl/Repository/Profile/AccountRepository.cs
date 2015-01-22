using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class AccountRepository : BaseOrgRepository, Core.Interface.Profile.IAccountRepository
    {
        #region attributes
        IEventLogRepository _logger;
       #endregion

        #region ctor
        public AccountRepository(IEventLogRepository logger)
        {
            _logger = logger;
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
            createOrg.Model.OrganizationType = accountOrgTypeId;
            createOrg.CreateOptions.ReturnModel = new Core.Models.Generated.Organization();

            CommerceCreateOperationResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest()).OperationResponses[0] as CommerceCreateOperationResponse;

            return new Guid(res.CommerceEntity.Id);
        }

        public Guid UpdateAccount(string name, Guid accountId) {
            KeithLink.Svc.Core.Models.Generated.SiteTerm orgTypes = GetOrganizationTypes();
            string accountOrgTypeId = orgTypes.Elements.Where(o => o.DisplayName == "Account").FirstOrDefault().Id;

            var createOrg = new CommerceServer.Foundation.CommerceUpdate<KeithLink.Svc.Core.Models.Generated.Organization>();
            createOrg.SearchCriteria.Model.Id = accountId.ToCommerceServerFormat();
            createOrg.Model.Name = name;

            createOrg.Model.OrganizationType = accountOrgTypeId;
            createOrg.UpdateOptions.ReturnModel = new Core.Models.Generated.Organization();

            CommerceUpdateOperationResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest()).OperationResponses[0] as CommerceUpdateOperationResponse;

            return new Guid(res.CommerceEntities[0].Id);
        }

        public List<Account> GetAccounts()
        {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '1'"; // org type of account

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var accounts = new System.Collections.Concurrent.BlockingCollection<Account>();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    KeithLink.Svc.Core.Models.Generated.Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    accounts.Add(new Account()
                    {
                        Id = Guid.Parse(org.Id),
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

        public void AddUserToAccount(Guid accountId, Guid userId)
        {
            base.AddUserToOrg(accountId, userId);
        }
        
        public void RemoveUserFromAccount(Guid accountId, Guid userId)
        {
            base.RemoveUserFromOrg(accountId, userId);
        }

        public List<Core.Models.Profile.Account> GetAccountsForUser(Guid userId)
        {
            // get user organization info
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();

            CommerceServer.Foundation.CommerceResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());

            List<Account> userAccounts = new List<Account>();
            foreach (CommerceEntity ent in (res.OperationResponses[0] as CommerceQueryOperationResponse).CommerceEntities)
            {
                Organization org = new Organization(ent);
                if (org.OrganizationType == "1")
                {
                    userAccounts.Add(new Account()
                    {
                        Id = Guid.Parse(org.Id),
                        Name = org.Name,
                    });
                }
            }
            return userAccounts;
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
            createOrg.Model.OrganizationType = "National ID"; // No National ID concept for now; only used in KBIT for reporting

            Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest());
        }

        #endregion


        public void RemoveCustomerFromAccount(Guid accountId, Guid customerId)
        {
            var updateQuery = new CommerceUpdate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            updateQuery.SearchCriteria.Model.Properties["Id"] = customerId.ToCommerceServerFormat();

            updateQuery.Model.ParentOrganizationId = string.Empty;

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());

            // TODO: remove all users associated directly to the customer
        }
    }
}
