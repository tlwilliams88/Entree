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
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Common.Core.AuditLog;
using KeithLink.Svc.Core.Interface.Invoices;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class CustomerRepository : BaseOrgRepository, Core.Interface.Profile.ICustomerRepository 
    {
        #region attributes

		protected string CACHE_GROUPNAME { get { return "Profile"; } }
		protected string CACHE_NAME { get { return "Profile"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }

        private readonly IEventLogRepository _logger;
        private readonly ICacheRepository _customerCacheRepository;
        private readonly IDsrServiceRepository _dsrService;
		private readonly IAuditLogRepository _auditLogRepository;
		private readonly IInvoiceServiceRepository _invoiceServiceRepository;

		

        #endregion

        #region ctor
		public CustomerRepository(IEventLogRepository logger, ICacheRepository customerCacheRepository, IDsrServiceRepository dsrService, IAuditLogRepository auditLogRepository, IInvoiceServiceRepository invoiceServiceRepository)
        {
            _logger = logger;
            _customerCacheRepository = customerCacheRepository;
            _dsrService = dsrService;
			_auditLogRepository = auditLogRepository;
			_invoiceServiceRepository = invoiceServiceRepository;
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
            queryOrg.SearchCriteria.WhereClause = "u_organization_type = '0'"; // org type of customer

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

        public void AddUserToCustomer(string addedBy, Guid customerId, Guid userId)
        {
            base.AddUserToOrg(customerId, userId);
			_customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
			_auditLogRepository.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserAssignedToCustomer, addedBy, string.Format("Customer: {0}, User: {1}", customerId, userId));
		}

        private static string GetUserOrgKey(Guid customerId, Guid userId)
        {
            return customerId.ToCommerceServerFormat() + "__" + userId.ToCommerceServerFormat();
        }

        public void RemoveUserFromCustomer(string removedBy, Guid customerId, Guid userId)
        {
            base.RemoveUserFromOrg(customerId, userId);
			_customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
			_auditLogRepository.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserRemovedFromCustomer, removedBy, string.Format("Customer: {0}, User: {1}", customerId, userId));
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
			queryOrg.SearchCriteria.WhereClause = "u_organization_type = '0' AND u_customer_number = '" + customerNumber + "' AND u_branch_number = '" + branchId + "'"; // org type of customer

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
            queryOrg.SearchCriteria.WhereClause = "u_name LIKE '%" + searchText.Replace("'", "''") + "%'"; // org type of customer

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
                // TODO - fill this in from real data source
                Phone = org.PreferredAddress != null 
                            && !String.IsNullOrEmpty(org.PreferredAddress.Telephone)
                            && !org.PreferredAddress.Telephone.Equals("0000000000") ? org.PreferredAddress.Telephone : string.Empty, // get from address profile
                Email = string.Empty,
                PointOfContact = string.Empty,
                TermCode = org.TermCode,
				KPayCustomer = org.AchType == "2" || org.AchType == "3",
				Dsr = dsrs == null || dsrs.Count == 0 ? null : dsrs.Where(d => d.Branch.Equals(org.BranchNumber, StringComparison.CurrentCultureIgnoreCase) && d.DsrNumber.Equals(org.DsrNumber)).DefaultIfEmpty(dsrs.Where(s => s.DsrNumber.Equals("000")).FirstOrDefault()).FirstOrDefault()
                , DsmNumber = org.DsmNumber
                , NationalId = org.NationalId
                , NationalNumber = org.NationalNumber
                , NationalSubNumber = org.NationalSubNumber
                , RegionalId = org.RegionalId
                , RegionalNumber = org.RegionalNumber
                , IsKeithNetCustomer = org.IsKeithnetCustomer !=null && org.IsKeithnetCustomer.ToLower() == "y" ? true : false
                
            };

			var term = _invoiceServiceRepository.ReadTermInformation(customer.CustomerBranch, customer.TermCode);
			if (term != null)
				customer.TermDescription = term.Description;

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
		
		//public List<Customer> GetCustomersForDSR(string dsrNumber, string branchId)
        public List<Customer> GetCustomersForDSR(List<Dsr> dsrList) {
            //var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsrNumber));
            List<Customer> customerFromCache = null;
            foreach (Dsr d in dsrList) {
                customerFromCache.AddRange(_customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(d.Branch + d.DsrNumber)));
            }
            
			if (customerFromCache != null)
				return customerFromCache;

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            //queryOrg.SearchCriteria.WhereClause = "u_dsr_number = '" + dsrNumber + "' AND u_branch_number = '" + branchId + "'"; // org type of customer

            System.Text.StringBuilder whereText = new System.Text.StringBuilder();
            for (int i = 0; i < dsrList.Count; i++) {
                if (i>0) {whereText.Append(" OR ");}
                whereText.AppendFormat("(u_branch_number = '{0}' AND u_dsr_number = '{1}')", dsrList[i].Branch, dsrList[i].DsrNumber);
            }
            queryOrg.SearchCriteria.WhereClause = whereText.ToString();

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

				//_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsrNumber), TimeSpan.FromHours(4), customers);
                foreach (Dsr d in dsrList) {
                    _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, 
                                                                     GetCacheKey(d.Branch + d.DsrNumber), 
                                                                     TimeSpan.FromHours(4), 
                                                                     (from Customer c in customers
                                                                      where c.CustomerBranch.Equals(d.Branch, StringComparison.InvariantCultureIgnoreCase) &&
                                                                            c.DsrNumber.Equals(d.DsrNumber, StringComparison.InvariantCultureIgnoreCase)
                                                                      select c
                                                                      ).ToList());
                }
			}

            return customers;
		}

        public List<Customer> GetCustomersForDSM(string dsmNumber, string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsmNumber));
            if (customerFromCache != null)
                return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "u_dsm_number = '" + dsmNumber + "' AND u_branch_number = '" + branchId + "'"; // org type of customer

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

                _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsmNumber), TimeSpan.FromHours(4), customers);
            }

            return customers;
        }

        public List<Customer> GetCustomersByNameSearchAndBranch(string search, string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(search + branchId));
            if (customerFromCache != null)
                return customerFromCache;

            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "u_branch_number = '" + branchId + "' AND u_name LIKE '%" + search.Replace("'", "''") + "%'"; // org type of customer

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
            queryOrg.SearchCriteria.WhereClause = "u_parent_organization = '" + accountId + "' AND u_organization_type = '0'"; // org type of customer

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
			queryOrg.SearchCriteria.WhereClause = "u_name LIKE '%" + search + "%' OR u_customer_number LIKE '%" + search + "%'";

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
			queryOrg.SearchCriteria.WhereClause = "u_parent_organization = '" + accountId + "'"; // org type of customer

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
            
            if (dsrInfo == null || dsrInfo.Count == 0) {
                _logger.WriteErrorLog("No DSRs returned from RetrieveDsrList in CustomerRepository, is BranchSupport.Dsrs populated?");
            }
			
            return dsrInfo;
		}


		public Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = string.Format("inner join [BEK_Commerce_profiles].[dbo].[UserOrganizationObject] uoo on oo.u_org_id = uoo.u_org_id WHERE uoo.u_user_id = '{0}' AND u_customer_number = '{1}' AND u_branch_number = '{2}'", userId.ToCommerceServerFormat(), customerNumber, branchId);

			
			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
				var dsrs = RetrieveDsrList();
				return OrgToCustomer(new KeithLink.Svc.Core.Models.Generated.Organization(res.CommerceEntities[0]), dsrs);
			}
			else
				return null;
		}

		#endregion

		#region Paged Results

		public PagedResults<Customer> GetPagedCustomers(int size, int from, string searchTerm)
		{
			var whereClause = "WHERE u_organization_type = '0'";
			
			return RetrievePagedResults(size, from, searchTerm, whereClause);
		}

		//public PagedResults<Customer> GetPagedCustomersForDSR(int size, int from, string dsrNumber, string branchId, string searchTerm)
        public PagedResults<Customer> GetPagedCustomersForDSR(int size, int from, string searchTerm, List<Dsr> dsrList)
		{
			PagedResults<Customer> returnValue = new PagedResults<Customer>();

            System.Text.StringBuilder whereText = new System.Text.StringBuilder();
            for (int i = 0; i < dsrList.Count; i++) {
                if (!String.IsNullOrEmpty(dsrList[i].DsrNumber) && !String.IsNullOrEmpty(dsrList[i].Branch))
                {
                    if (i > 0) { whereText.Append(" OR "); }
                    whereText.AppendFormat("(u_branch_number = '{0}' AND u_dsr_number = '{1}')", dsrList[i].Branch, dsrList[i].DsrNumber);
                }
            }
            
            if (!String.IsNullOrEmpty(whereText.ToString()))
            {
                returnValue = RetrievePagedResults(size, from, searchTerm, whereText.ToString());
            }
                        
            return returnValue;
		}

		public PagedResults<Customer> GetPagedCustomersForDSM(int size, int from, string dsrNumber, string branchId, string searchTerm)
		{
			var whereClause = string.Format("WHERE u_organization_type = '0' AND u_dsm_number = '{0}' AND u_branch_number = '{1}'", dsrNumber, branchId);

			return RetrievePagedResults(size, from, searchTerm, whereClause);
		}

		public PagedResults<Customer> GetPagedCustomersForBranch(int size, int from, string branchId, string searchTerm)
		{
			var whereClause = string.Format("WHERE u_organization_type = '0' AND u_branch_number = '{0}'", branchId);

			return RetrievePagedResults(size, from, searchTerm, whereClause);
		}

		public PagedResults<Customer> GetPagedCustomersForUser(int size, int from, Guid userId, string searchTerm)
		{
			var whereClause = string.Format("inner join [BEK_Commerce_profiles].[dbo].[UserOrganizationObject] uoo on oo.u_org_id = uoo.u_org_id WHERE uoo.u_user_id = '{0}' and u_organization_type = '0'", userId.ToCommerceServerFormat());

			return RetrievePagedResults(size, from, searchTerm, whereClause);
		}

		public PagedResults<Customer> GetPagedCustomersForAccount(int size, int from, string searchTerm, string accountId)
		{
			var whereClause = string.Format("WHERE u_organization_type = '0' AND u_parent_organization = '{0}'", accountId);

			return RetrievePagedResults(size, from, searchTerm, whereClause);
		}

		private PagedResults<Customer> RetrievePagedResults(int size, int from, string searchTerm, string whereClause)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

			if (!string.IsNullOrEmpty(searchTerm))
				whereClause += " AND (u_customer_number LIKE '%" + searchTerm.Replace("'", "''") + "%' OR u_name LIKE '%" + searchTerm.Replace("'", "''") + "%')"; // org type of customer


			queryOrg.SearchCriteria.WhereClause = whereClause;
			queryOrg.SearchCriteria.FirstItemIndex = from;
			queryOrg.SearchCriteria.NumberOfItemsToReturn = size;
			queryOrg.SearchCriteria.ReturnTotalItemCount = true;

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
			var dsrs = RetrieveDsrList();
			System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
			{
				Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
				customers.Add(OrgToCustomer(org, dsrs));
			});

			return new PagedResults<Customer>() { Results = customers.ToList(), TotalResults = res.TotalItemCount.HasValue ? res.TotalItemCount.Value : 0 };
		}
		
		#endregion
				
		public Customer GetCustomerById(Guid customerId)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "u_org_id = '" + customerId.ToCommerceServerFormat() + "'";

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
