using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class CustomerRepository : BaseOrgRepository, Core.Interface.Profile.ICustomerRepository
    {
        #region attributes
        IEventLogRepository _logger;
        ICustomerCacheRepository _customerCacheRepository;
        #endregion

        #region ctor
        public CustomerRepository(IEventLogRepository logger, ICustomerCacheRepository customerCacheRepository)
        {
            _logger = logger;
            _customerCacheRepository = customerCacheRepository;
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
        public List<Customer> GetCustomers() {
            var allCustomersFromCache = _customerCacheRepository.GetItem<List<Customer>>(GetCacheKey("allCustomers"));
            if (allCustomersFromCache != null)
                return allCustomersFromCache;
                
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '0'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    customers.Add(OrgToCustomer(org));
                });

            List<Customer> customersList = customers.ToList();
            _customerCacheRepository.AddItem<List<Customer>>(GetCacheKey("allCustomers"), customersList);
            return  customersList;
        }

        public void ClearCustomerCache()
        {
            _customerCacheRepository.RemoveItem(GetCacheKey("allCustomers"));
        }

        public void AddUserToCustomer(Guid customerId, Guid userId)
        {
            base.AddUserToOrg(customerId, userId);
        }

        private static string GetUserOrgKey(Guid customerId, Guid userId)
        {
            return customerId.ToCommerceServerFormat() + "__" + userId.ToCommerceServerFormat();
        }

        public void RemoveUserFromCustomer(Guid customerId, Guid userId)
        {
            base.RemoveUserFromOrg(customerId, userId);
        }

        public List<Core.Models.Profile.Customer> GetCustomersForUser(Guid userId)
        {
            // get user organization info
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();

            CommerceServer.Foundation.CommerceResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());

            List<Customer> userCustomers = new List<Customer>();
            foreach (CommerceEntity ent in (res.OperationResponses[0] as CommerceQueryOperationResponse).CommerceEntities)
            {
                Organization org = new Organization(ent);
                if (org.OrganizationType == "0")
                {
                    userCustomers.Add(OrgToCustomer(org));
                }
            }
            return userCustomers;
        }

        protected string GetCacheKey(string setName)
        {
            return "CustomerCache_" + setName;
        }

        public Customer GetCustomerByCustomerNumber(string customerNumber)
        {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '0' AND GeneralInfo.customer_number = '" + customerNumber + "'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            if (res.CommerceEntities.Count > 0)
                return OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
            else
                return null;
        }

        public List<Customer> GetCustomersByNameSearch(string searchText)
        {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.name LIKE '%" + searchText + "%'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            List<Customer> customers = new List<Customer>();
            foreach (CommerceEntity ent in res.CommerceEntities)
            {
                Organization org = new Organization(ent);
                if (org.OrganizationType == "0")
                {
                    customers.Add(OrgToCustomer(org));
                }
            }
            return customers;
        }

        private static Customer OrgToCustomer(Organization org)
        {
            return new Customer()
            {
                CustomerId = Guid.Parse(org.Id),
                AccountId = String.IsNullOrEmpty(org.ParentOrganizationId) ? new Nullable<Guid>() : Guid.Parse(org.ParentOrganizationId),
                ContractId = org.ContractNumber,
                CustomerBranch = org.BranchNumber,
                CustomerName = org.Name,
                CustomerNumber = org.CustomerNumber,
                DsrNumber = org.DsrNumber,
                IsPoRequired = org.IsPoRequired.HasValue ? org.IsPoRequired.Value : false,
                IsPowerMenu = org.IsPowerMenu.HasValue ? org.IsPowerMenu.Value : false,
                NationalId = org.NationalAccountId,
                // TODO - fill this in from real data source
                Phone = "303-422-7765",
                Email = "test@test.com",
                PointOfContact = "test@test.com",
                Address = new Address() { StreetAddress = "2102 East St", City = "Golden", RegionCode = "CO", PostalCode = "80401" },
                CurrentBalance = org.CurrentBalance,
                BalanceAge1 = org.BalanceAge1,
                BalanceAge2 = org.BalanceAge2,
                BalanceAge3 = org.BalanceAge3,
                BalanceAge4 = org.BalanceAge4,
                TermCode = org.TermCode
            };
        }

        #endregion

    }
}
