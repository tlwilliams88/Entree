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
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(GetCacheKey(string.Format("user_{0}",userId.ToString())));
			if (customerFromCache != null && customerFromCache.Count > 0)
				return customerFromCache;

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
			_customerCacheRepository.AddItem<List<Customer>>(GetCacheKey(string.Format("user_{0}", userId.ToString())), userCustomers);

            return userCustomers;
        }

        protected string GetCacheKey(string setName)
        {
            return "CustomerCache_" + setName;
        }

        public Customer GetCustomerByCustomerNumber(string customerNumber)
        {
			var customerFromCache = _customerCacheRepository.GetItem<Customer>(GetCacheKey(customerNumber));
			if (customerFromCache != null)
				return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '0' AND GeneralInfo.customer_number = '" + customerNumber + "'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
				_customerCacheRepository.AddItem<Customer>(GetCacheKey(customerNumber), customer);

				return customer;
			}
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
            Customer customer =  new Customer()
            {
                CustomerId = Guid.Parse(org.Id),
                AccountId = String.IsNullOrEmpty(org.ParentOrganizationId) ? new Nullable<Guid>() : Guid.Parse(org.ParentOrganizationId),
                ContractId = org.ContractNumber,
                DisplayName = string.Format("{0} - {1}", org.CustomerNumber, org.Name),
                CustomerBranch = org.BranchNumber,
                CustomerName = org.Name,
                CustomerNumber = org.CustomerNumber,
                DsrNumber = org.DsrNumber,
                IsPoRequired = org.IsPoRequired.HasValue ? org.IsPoRequired.Value : false,
                IsPowerMenu = org.IsPowerMenu.HasValue ? org.IsPowerMenu.Value : false,
                NationalId = org.NationalAccountId,
                // TODO - fill this in from real data source
                Phone = org.PreferredAddress != null 
                            && !String.IsNullOrEmpty(org.PreferredAddress.Telephone)
                            && !org.PreferredAddress.Telephone.Equals("0000000000") ? org.PreferredAddress.Telephone : string.Empty, // get from address profile
                Email = string.Empty,
                PointOfContact = string.Empty,
                CurrentBalance = org.CurrentBalance,
                BalanceAge1 = org.BalanceAge1,
                BalanceAge2 = org.BalanceAge2,
                BalanceAge3 = org.BalanceAge3,
                BalanceAge4 = org.BalanceAge4,
                TermCode = org.TermCode,
                KPayCustomer = org.AchType == "2"
            };

            // fill in the address
            customer.Address = org.PreferredAddress != null ? new Address()
                    {
                        StreetAddress =
                            !String.IsNullOrEmpty(org.PreferredAddress.Line1) && !String.IsNullOrEmpty(org.PreferredAddress.Line2)
                            ? org.PreferredAddress.Line1 + System.Environment.NewLine + org.PreferredAddress.Line2
                            : !String.IsNullOrEmpty(org.PreferredAddress.Line1) ? org.PreferredAddress.Line1 : string.Empty,
                        City = !String.IsNullOrEmpty(org.PreferredAddress.City) ? org.PreferredAddress.City : string.Empty,
                        RegionCode = !String.IsNullOrEmpty(org.PreferredAddress.StateProvinceCode) ? org.PreferredAddress.StateProvinceCode : string.Empty,
                        PostalCode = !String.IsNullOrEmpty(org.PreferredAddress.ZipPostalCode) ? org.PreferredAddress.ZipPostalCode : string.Empty
                    }
                    : new Address() { StreetAddress = string.Empty, City = string.Empty, RegionCode = string.Empty, PostalCode = string.Empty };

            return customer;
        }

        #endregion



		public List<Customer> GetCustomersForDSR(string dsrNumber)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(GetCacheKey(dsrNumber));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.dsr_number = '" + dsrNumber + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				List<Customer> customers = new List<Customer>();
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org));
					}
				}

				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
				_customerCacheRepository.AddItem<List<Customer>>(GetCacheKey(dsrNumber), customers);

				return customers;
			}
			else
				return null;
		}

		public List<Customer> GetCustomersForAccount(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.national_account_id = '" + accountId + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				List<Customer> customers = new List<Customer>();
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org));
					}
				}

				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
				_customerCacheRepository.AddItem<List<Customer>>(GetCacheKey(string.Format("acct-{0}", accountId)), customers);

				return customers;
			}
			else
				return null;
		}


		public List<Customer> GetCustomersByNameOrNumber(string search)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.name LIKE '%" + search + "%' OR GeneralInfo.customer_number LIKE '%" + search + "%'";

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				List<Customer> customers = new List<Customer>();
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org));
					}
				}

				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
				
				return customers;
			}
			else
				return null;
		}


		public List<Customer> GetCustomersForParentAccountOrganization(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.parent_organization = '" + accountId + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				List<Customer> customers = new List<Customer>();
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org));
					}
				}

				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]));
				_customerCacheRepository.AddItem<List<Customer>>(GetCacheKey(string.Format("acct-{0}", accountId)), customers);

				return customers;
			}
			else
				return null;
		}
	}
}
