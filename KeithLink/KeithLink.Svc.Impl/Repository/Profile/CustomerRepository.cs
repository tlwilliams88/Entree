using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class CustomerRepository : BaseOrgRepository, Core.Interface.Profile.ICustomerRepository 
    {
        #region attributes

		protected string CACHE_GROUPNAME { get { return "Profile"; } }
		protected string CACHE_NAME { get { return "Profile"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }

        IEventLogRepository _logger;
        ICacheRepository _customerCacheRepository;
        IDsrServiceRepository _dsrService;

		

        #endregion

        #region ctor
		public CustomerRepository(IEventLogRepository logger, ICacheRepository customerCacheRepository, IDsrServiceRepository dsrService)
        {
            _logger = logger;
            _customerCacheRepository = customerCacheRepository;
            _dsrService = dsrService;
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
            var allCustomersFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("allCustomers"));
            if (allCustomersFromCache != null)
                return allCustomersFromCache;
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '0'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
			var dsrs = RetrieveDsrList();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    customers.Add(OrgToCustomer(org, dsrs));
                });

            List<Customer> customersList = customers.ToList();
			_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("allCustomers"), TimeSpan.FromHours(4), customersList);
            return  customersList;
        }

        public void ClearCustomerCache()
        {
			_customerCacheRepository.ResetAllItems(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME);
        }

        public void AddUserToCustomer(Guid customerId, Guid userId)
        {
            base.AddUserToOrg(customerId, userId);
			_customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
		}

        private static string GetUserOrgKey(Guid customerId, Guid userId)
        {
            return customerId.ToCommerceServerFormat() + "__" + userId.ToCommerceServerFormat();
        }

        public void RemoveUserFromCustomer(Guid customerId, Guid userId)
        {
            base.RemoveUserFromOrg(customerId, userId);
			_customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
        }

        public List<Core.Models.Profile.Customer> GetCustomersForUser(Guid userId)
        {
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
			if (customerFromCache != null && customerFromCache.Count > 0)
				return customerFromCache;

            // get user organization info
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();

            CommerceServer.Foundation.CommerceResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(profileQuery.ToRequest());

            List<Customer> userCustomers = new List<Customer>();
			var dsrs = RetrieveDsrList();
            foreach (CommerceEntity ent in (res.OperationResponses[0] as CommerceQueryOperationResponse).CommerceEntities)
            {
                Organization org = new Organization(ent);
                if (org.OrganizationType == "0")
                {
                    userCustomers.Add(OrgToCustomer(org, dsrs));
                }
            }
			_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())), TimeSpan.FromHours(4), userCustomers);

            return userCustomers;
        }

        protected string GetCacheKey(string setName)
        {
            return "CustomerCache_" + setName;
        }

		public Customer GetCustomerByCustomerNumber(string customerNumber, string branchId)
        {
			var customerFromCache = _customerCacheRepository.GetItem<Customer>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("{0}-{1}",customerNumber, branchId)));
			if (customerFromCache != null)
				return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.organization_type = '0' AND GeneralInfo.customer_number = '" + customerNumber + "' AND GeneralInfo.branch_number = '" + branchId + "'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				var dsrs = RetrieveDsrList();
				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]), dsrs);
				_customerCacheRepository.AddItem<Customer>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("{0}-{1}", customerNumber, branchId)), TimeSpan.FromHours(4), customer);

				return customer;
			}
			else
				return null;
        }

        public List<Customer> GetCustomersByNameSearch(string searchText)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(searchText));
            if (customerFromCache != null)
                return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.name LIKE '%" + searchText.Replace("'", "''") + "%'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
            foreach (CommerceEntity ent in res.CommerceEntities)
            {
                Organization org = new Organization(ent);
                if (org.OrganizationType == "0")
                {
                    customers.Add(OrgToCustomer(org, dsrs));
                }
            }

            _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(searchText), TimeSpan.FromMinutes(2), customers);
            return customers;
        }

        private Customer OrgToCustomer(Organization org, List<Dsr> dsrs)
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
                TermCode = org.TermCode,
				KPayCustomer = org.AchType == "2" || org.AchType == "3",
				Dsr = dsrs.Where(d => d.Branch.Equals(org.BranchNumber, StringComparison.CurrentCultureIgnoreCase) && d.DsrNumber.Equals(org.DsrNumber)).FirstOrDefault()
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
		
		public List<Customer> GetCustomersForDSR(string dsrNumber)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsrNumber));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.dsr_number = '" + dsrNumber + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
			if (res.CommerceEntities.Count > 0)
			{
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org, dsrs));
					}
				}

				_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsrNumber), TimeSpan.FromHours(4), customers);
			}

            return customers;
		}

        public List<Customer> GetCustomersByNameSearchAndBranch(string search, string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(search + branchId));
            if (customerFromCache != null)
                return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.branch_number = '" + branchId + "' AND GeneralInfo.name LIKE '%" + search.Replace("'", "''") + "%'"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
            if (res.CommerceEntities.Count > 0)
            {
                foreach (CommerceEntity ent in res.CommerceEntities)
                {
                    Organization org = new Organization(ent);
                    if (org.OrganizationType == "0")
                    {
                        customers.Add(OrgToCustomer(org, dsrs));
                    }
                }

                _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(search + branchId), TimeSpan.FromHours(4), customers);
            }

            return customers;
        }

		public List<Customer> GetCustomersForAccount(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "GeneralInfo.parent_organization = '" + accountId + "' AND GeneralInfo.organization_type = '0'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
        	List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
			if (res.CommerceEntities.Count > 0)
			{
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org, dsrs));
					}
				}

				_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)), TimeSpan.FromHours(4), customers);
			}

            return customers;
		}

		public List<Customer> GetCustomersByNameOrNumber(string search)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.name LIKE '%" + search + "%' OR GeneralInfo.customer_number LIKE '%" + search + "%'";

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
			if (res.CommerceEntities.Count > 0)
			{
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org, dsrs));
					}
				}
			}
            return customers;
        }

		public List<Customer> GetCustomersForParentAccountOrganization(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.parent_organization = '" + accountId + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> customers = new List<Customer>();
			var dsrs = RetrieveDsrList();
			if (res.CommerceEntities.Count > 0)
			{
				foreach (CommerceEntity ent in res.CommerceEntities)
				{
					Organization org = new Organization(ent);
					if (org.OrganizationType == "0")
					{
						customers.Add(OrgToCustomer(org, dsrs));
					}
				}

				_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)), TimeSpan.FromHours(4), customers);
			}

            return customers;
		}

		private List<Dsr> RetrieveDsrList()
		{
			//In the interest of time, and because we have a production deploy that needs to happen right away, just retrieve all dsrs and cache the result
			//The number of dsrs is pretty small, so this solution can stay like this, or it can be modified to look up a specific set of dsrs
			//But with it being cached, and the small number, this is likely better performance once the initial load occurs

			var cachedallDsrInfo = _customerCacheRepository.GetItem<List<Dsr>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("dsrInfo"));
			if (cachedallDsrInfo != null)
				return cachedallDsrInfo;

			var dsrInfo = _dsrService.GetAllDsrInfo();
			//Cache the dsrs
			_customerCacheRepository.AddItem<List<Dsr>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("dsrInfo"), TimeSpan.FromHours(4), dsrInfo);

			return dsrInfo;
		}
		

		#endregion


		public Customer GetCustomerById(Guid customerId)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "GeneralInfo.org_id = '" + customerId.ToCommerceServerFormat() + "'";

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				var dsrs = RetrieveDsrList();
				var customer = OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]), dsrs);
				
				return customer;
			}
			else
				return null;
		}
	}
}
