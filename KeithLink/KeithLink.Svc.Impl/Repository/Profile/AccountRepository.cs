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
using KeithLink.Common.Core.AuditLog;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class AccountRepository : BaseOrgRepository, Core.Interface.Profile.IAccountRepository
    {
        #region attributes
        IEventLogRepository _logger;
		private readonly IAuditLogRepository _auditLog;
       #endregion

        #region ctor
        public AccountRepository(IEventLogRepository logger, IAuditLogRepository auditLog)
        {
            _logger = logger;
			_auditLog = auditLog;
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
		public Guid CreateAccount(string createdBy, string name)
		{
            KeithLink.Svc.Core.Models.Generated.SiteTerm orgTypes = GetOrganizationTypes();
            string accountOrgTypeId = orgTypes.Elements.Where(o => o.DisplayName == "Account").FirstOrDefault().Id;

            var createOrg = new CommerceServer.Foundation.CommerceCreate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            createOrg.Model.Name = name;
            createOrg.Model.OrganizationType = accountOrgTypeId;
            createOrg.CreateOptions.ReturnModel = new Core.Models.Generated.Organization();

            CommerceCreateOperationResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest()).OperationResponses[0] as CommerceCreateOperationResponse;
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.CustomerGroupCreated, createdBy, name);
            return new Guid(res.CommerceEntity.Id);
        }

		public void DeleteAccount(string deletedBy, Guid accountId)
		{
			var deleteOrg = new CommerceServer.Foundation.CommerceDelete<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

			deleteOrg.SearchCriteria.Model.Id = accountId.ToCommerceServerFormat();
			deleteOrg.DeleteOptions.ReturnDeletedCount = true;

			CommerceCreateOperationResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(deleteOrg.ToRequest()).OperationResponses[0] as CommerceCreateOperationResponse;
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.CustomerGroupDeleted, deletedBy, accountId.ToString());
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
            queryOrg.SearchCriteria.WhereClause = "u_organization_type = '1'"; // org type of account

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

        public void AddCustomerToAccount(string addedBy, Guid accountId, Guid customerId)
        {
            var updateQuery = new CommerceUpdate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            updateQuery.SearchCriteria.Model.Properties["Id"] = customerId.ToCommerceServerFormat();

            updateQuery.Model.ParentOrganizationId = accountId.ToCommerceServerFormat();

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.CustomerAddedToCustomerGroup, addedBy, string.Format("Customer: {0}, Account: {1}", customerId, accountId));
        }

        public void AddUserToAccount(string addedBy, Guid accountId, Guid userId)
        {
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserAddedToCustomerGroup, addedBy, string.Format("Account: {0}, User: {1}", accountId, userId));
            base.AddUserToOrg(accountId, userId);
        }
        
        public void RemoveUserFromAccount(string removedBy, Guid accountId, Guid userId)
        {
			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserRemovedFromCustomerGroup, removedBy, string.Format("Account: {0}, User: {1}", accountId, userId));
            
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


        public void RemoveCustomerFromAccount(string removedBy, Guid accountId, Guid customerId)
        {
            var updateQuery = new CommerceUpdate<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            updateQuery.SearchCriteria.Model.Properties["Id"] = customerId.ToCommerceServerFormat();

            updateQuery.Model.ParentOrganizationId = string.Empty;

            var response = FoundationService.ExecuteRequest(updateQuery.ToRequest());

			_auditLog.WriteToAuditLog(Common.Core.Enumerations.AuditType.CustomerRemovedFromCustomerGroup, removedBy, string.Format("Customer: {0}, Account: {1}", customerId, accountId));
            // TODO: remove all users associated directly to the customer
        }
    }
}
