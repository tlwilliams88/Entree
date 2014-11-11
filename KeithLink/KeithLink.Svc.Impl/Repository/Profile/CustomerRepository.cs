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
                
            var createOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            createOrg.SearchCriteria.Model.OrganizationType = "0"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    customers.Add(new Customer()
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
                        NationalId = org.NationalAccountId
                    });
                });

            List<Customer> customersList = customers.ToList();
            _customerCacheRepository.AddItem<List<Customer>>(GetCacheKey("allCustomers"), customersList);
            return  customersList;
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
                    userCustomers.Add(new Customer()
                    {
                        CustomerId = Guid.Parse(org.Id),
                        CustomerName = org.Name,
                        CustomerNumber = org.CustomerNumber,
                        CustomerBranch = org.BranchNumber.ToLower(),
                        ContractId = org.ContractNumber,
                        DsrNumber = org.DsrNumber,
                        IsPoRequired = org.IsPoRequired.HasValue ? org.IsPoRequired.Value : false,
                        IsPowerMenu = org.IsPowerMenu.HasValue ? org.IsPowerMenu.Value : false,
                        NationalOrRegionalAccountNumber = org.NationalOrRegionalAccountNumber,
                        AccountId = !String.IsNullOrEmpty(org.ParentOrganizationId) ? Guid.Parse(org.ParentOrganizationId) : new Nullable<Guid>(),
                        // TODO - fill this in from real data source
                        Phone = "303-422-7765",
                        Email = "test@test.com",
                        PointOfContact = "test@test.com",
                        Address = new Address() { StreetAddress = "2102 East St", City = "Golden", RegionCode = "CO", PostalCode = "80401" }
                    });
                }
            }
            return userCustomers;
        }

        protected string GetCacheKey(string setName)
        {
            return "CustomerCache_" + setName;
        }
        #endregion
    }
}
