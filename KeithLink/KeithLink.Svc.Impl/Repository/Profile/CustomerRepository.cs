using CommerceServer.Foundation;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Profile;

using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class CustomerRepository : BaseOrgRepository, ICustomerRepository 
    {
        #region attributes

		protected string CACHE_GROUPNAME { get { return "Profile"; } }
		protected string CACHE_NAME { get { return "Profile"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }

        private const string ORGANIZATION_TYPE_CUSTOMER = "0";
        private const string ORGANIZATION_TYPE_ACCOUNTGROUP = "1";

        private const string DEFAULT_DSR_NUMBER = "000";

        private readonly IEventLogRepository _logger;
        private readonly ICacheRepository _customerCacheRepository;
        private readonly IDsrLogic _dsrLogic;
		private readonly IAuditLogRepository _auditLogRepository;
        private readonly ITermRepository _termRepo;
        #endregion

        #region ctor
		public CustomerRepository(IEventLogRepository logger, ICacheRepository customerCacheRepository, IDsrLogic dsrLogic, 
                                  IAuditLogRepository auditLogRepository, ITermRepository termRepository) {
            _logger = logger;
            _customerCacheRepository = customerCacheRepository;
            _dsrLogic = dsrLogic;
			_auditLogRepository = auditLogRepository;
            _termRepo = termRepository;
        }
        #endregion

        #region methods
        public void AddUserToCustomer(string addedBy, Guid customerId, Guid userId) {
            base.AddUserToOrg(customerId, userId);
            _customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
            _auditLogRepository.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserAssignedToCustomer, addedBy, string.Format("Customer: {0}, User: {1}", customerId, userId));
        }

        /// <summary>
        /// build the where clause for the RetrievePagedList method
        /// </summary>
        /// <param name="existingWhereClause">the where clause that is being built up for querying customers</param>
        /// <param name="searchTerm">the terms that we are using to search</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>the complete where clause to search for customers</returns>
        private string BuildWhereClauseForCustomerSearch(string existingWhereClause, string searchTerm, CustomerSearchType searchType) {
            StringBuilder whereClause = new StringBuilder();
            
            if(!string.IsNullOrEmpty(existingWhereClause)) { whereClause.Append(existingWhereClause); }

            if (!string.IsNullOrEmpty(searchTerm)) {
                if (whereClause.Length == 0) {
                    whereClause.Append(" WHERE ");
                } else {
                    whereClause.Append(" AND ");
                }

                int tempSearchNumber = 0;
                bool isNumber = int.TryParse(searchTerm, out tempSearchNumber);
                
                switch (searchType) {
                    case CustomerSearchType.NationalAccount:
                        if (isNumber) {
                            if (searchTerm.Length > 2) {
                                // search by national account number and sub number
                                whereClause.Append(string.Format("(u_national_number = '{0}' AND u_national_sub_number ='{1}')", searchTerm.Substring(0, 2), searchTerm.Substring(2)));
                            } else {
                                // search only by national account number
                                whereClause.Append("u_national_number = '{SearchTerm}'");
                            }
                        } else {
                            // search by national id or national id description
                            whereClause.Append("(u_national_id_desc LIKE '%{SearchTerm}%' " +
                                               "OR u_national_id LIKE '%{SearchTerm}%')");

                        }
                        
                        break;
                    case CustomerSearchType.RegionalAccount:
                        if (isNumber) {
                            whereClause.Append("u_regional_number = '{SearchTerm}'");
                        } else {
                            whereClause.Append("(u_regional_id_desc LIKE '%{SearchTerm}%' " +
                                               "OR u_regional_id LIKE '%{SearchTerm}%')");
                        }

                        break;
                    case CustomerSearchType.Customer:
                    default:
                        whereClause.Append("(u_customer_number LIKE '%{SearchTerm}%' OR u_name LIKE '%{SearchTerm}%')");
                        break;
                }
            }

            if (whereClause.Length == 0){
                return string.Empty;
            } else {
                string terms = searchTerm.Replace("'", "''");

                return whereClause.ToString().InjectSingleValue("SearchTerm", terms);
            }
        }

        private List<Customer> BuildCustomerList(List<CommerceEntity> organizations) {
            var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
            Dictionary<string, Dsr> dsrDict = RetrieveDsrDictionary();
            Dictionary<string, string> termDict = RetrieveTermsCodeDict();

            System.Threading.Tasks.Parallel.ForEach(organizations, e => {
                Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);

                if(org.OrganizationType == ORGANIZATION_TYPE_CUSTOMER) {
                    Customer myCustomer = org.ToCustomer();

                    string termKey = GetTermKey(org.BranchNumber, org.TermCode);

                    if(termDict.ContainsKey(termKey)) {
                        myCustomer.TermDescription = termDict[termKey];
                    }

                    string dsrKey = GetDsrKey(myCustomer.CustomerBranch, myCustomer.DsrNumber);
                    Dsr myDsr = null;

                    if(dsrDict.ContainsKey(dsrKey)) {
                        myDsr = dsrDict[dsrKey];
                    } else {
                        dsrKey = GetDsrKey(myCustomer.CustomerBranch, DEFAULT_DSR_NUMBER);
                        if(dsrDict.ContainsKey(dsrKey)) {
                            myDsr = dsrDict[dsrKey];
                        }
                    }

                    myCustomer.Dsr = myDsr;

                    customers.Add(myCustomer);
                }
            });

            return customers.ToList();
        }

        public void ClearCustomerCache() {
            _customerCacheRepository.ResetAllItems(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME);
        }

		public Customer GetCustomerByCustomerNumber(string customerNumber, string branchId)
        {
			var customerFromCache = _customerCacheRepository.GetItem<Customer>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("{0}-{1}",customerNumber, branchId)));
			if (customerFromCache == null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_organization_type = '0' AND u_customer_number = '" + customerNumber + "' AND u_branch_number = '" + branchId + "'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

                if(res.CommerceEntities.Count > 0) {
                    List<Customer> results = BuildCustomerList(res.CommerceEntities);

                    _customerCacheRepository.AddItem<Customer>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("{0}-{1}", customerNumber, branchId)), TimeSpan.FromHours(4), results[0]);

                    return results[0];
                } else
                    return null;
            } else {
				return customerFromCache;
            }
        }

        public Customer GetCustomerById(Guid customerId) {
            var queryOrg = new CommerceQuery<Organization>("Organization");
            queryOrg.SearchCriteria.WhereClause = "u_org_id = '" + customerId.ToCommerceServerFormat() + "'";

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            if (res.CommerceEntities.Count > 0) {
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                return results[0];
            } else
                return null;
        }

		public Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = string.Format("inner join [BEK_Commerce_profiles].[dbo].[UserOrganizationObject] uoo on oo.u_org_id = uoo.u_org_id WHERE uoo.u_user_id = '{0}' AND u_customer_number = '{1}' AND u_branch_number = '{2}'", userId.ToCommerceServerFormat(), customerNumber, branchId);

			
			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

			if (res.CommerceEntities.Count > 0)
			{
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                return results[0];
            }
			else
				return null;
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
        public List<Customer> GetCustomers() {
            var allCustomersFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("allCustomers"));
            if (allCustomersFromCache == null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_organization_type = '0'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("allCustomers"), TimeSpan.FromHours(4), results);

                return results;
            } else {
                return allCustomersFromCache;
            }
        }

		public List<Customer> GetCustomersByNameOrNumber(string search)
		{
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "u_name LIKE '%" + search + "%' OR u_customer_number LIKE '%" + search + "%'";

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> results = BuildCustomerList(res.CommerceEntities);

            return results;
        }
        
        public List<Customer> GetCustomersByNameSearch(string searchText)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(searchText));
            if (customerFromCache == null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_name LIKE '%" + searchText.Replace("'", "''") + "%'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(searchText), TimeSpan.FromMinutes(2), results);

                return results;
            } else {
                return customerFromCache;
            }
        }
        
        public List<Customer> GetCustomersByNameSearchAndBranch(string search, string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(search + branchId));
            if (customerFromCache != null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_branch_number = '" + branchId + "' AND u_name LIKE '%" + search.Replace("'", "''") + "%'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                if(results.Count > 0) {
                    _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(search + branchId), TimeSpan.FromHours(4), results);
                }

                return results;
            } else {
                return customerFromCache;
            }
        }
        
		public List<Customer> GetCustomersForAccount(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache == null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_parent_organization = '" + accountId + "' AND u_organization_type = '0'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                if(results.Count > 0) {
                    _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)), TimeSpan.FromHours(4), results);
                }

                return results;
            } else {
				return customerFromCache;
            }
		}

        public List<Customer> GetCustomersForDSR(List<Dsr> dsrList) {
			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            System.Text.StringBuilder whereText = new System.Text.StringBuilder();
            for (int i = 0; i < dsrList.Count; i++) {
                if (i>0) {whereText.Append(" OR ");}
                whereText.AppendFormat("(u_branch_number = '{0}' AND u_dsr_number = '{1}')", dsrList[i].Branch, dsrList[i].DsrNumber);
            }
            queryOrg.SearchCriteria.WhereClause = whereText.ToString();

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> results = BuildCustomerList(res.CommerceEntities);

            return results;
		}

        public List<Customer> GetCustomersForBranch(string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("Branch=" + branchId));
            if (customerFromCache == null)
            {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_branch_number = '" + branchId + "'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                if (results.Count > 0)
                {
                    _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("Branch=" + branchId), TimeSpan.FromHours(4), results);
                }

                return results;
            }
            else {
                return customerFromCache;
            }
        }

        public List<Customer> GetCustomersForDSM(string dsmNumber, string branchId)
        {
            var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsmNumber));
            if (customerFromCache == null) {
                var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
                queryOrg.SearchCriteria.WhereClause = "u_dsm_number = '" + dsmNumber + "' AND u_branch_number = '" + branchId + "'"; // org type of customer

                CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
                List<Customer> results = BuildCustomerList(res.CommerceEntities);

                if(results.Count > 0) {
                    _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(dsmNumber), TimeSpan.FromHours(4), results);
                }

                return results;
            } else {
                return customerFromCache;
            }
        }
        
		public List<Customer> GetCustomersForParentAccountOrganization(string accountId)
		{
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)));
			if (customerFromCache == null) {

            } else {
				return customerFromCache;
            }

			var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
			queryOrg.SearchCriteria.WhereClause = "u_parent_organization = '" + accountId + "'"; // org type of customer

			CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> results = BuildCustomerList(res.CommerceEntities);

			if (results.Count > 0) {
				_customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("acct-{0}", accountId)), TimeSpan.FromHours(4), results);
			}

            return results;
		}
        
        public List<Customer> GetCustomersForUser(Guid userId)
        {
			var customerFromCache = _customerCacheRepository.GetItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
			if (customerFromCache != null && customerFromCache.Count > 0) {

            //} else {
				return customerFromCache;
            }

            // get user organization info
            var profileQuery = new CommerceServer.Foundation.CommerceQuery<CommerceServer.Foundation.CommerceEntity>("UserOrganizations");
            profileQuery.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();

            CommerceQueryOperationResponse res = (FoundationService.ExecuteRequest(profileQuery.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> results = BuildCustomerList(res.CommerceEntities);

            _customerCacheRepository.AddItem<List<Customer>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())), TimeSpan.FromHours(4), results);

            return results;
        }

        private string GetDsrKey(string branchId, string dsrNumber) {
            StringBuilder output = new StringBuilder();

            output.Append(branchId.ToString());
            output.Append("-");
            output.Append(dsrNumber);

            return output.ToString();
        }

        /// <summary>
        /// Find customers and put the results into a paged list
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="searchTerm"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public PagedResults<Customer> GetPagedCustomers(PagingModel paging, string searchTerm, CustomerSearchType searchType)
		{
			var whereClause = "WHERE u_organization_type = '0'";

			return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
		}

        /// <summary>
        /// Search for customers that belong to the specified account
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="accountId">the account that the search is limited to</param>
        /// <param name="searchType">seach on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        public PagedResults<Customer> GetPagedCustomersForAccount(PagingModel paging, string searchTerm, string accountId, CustomerSearchType searchType) {
            var whereClause = string.Format("WHERE u_organization_type = '0' AND u_parent_organization = '{0}'", accountId);

            return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
        }

        /// <summary>
        /// Search for customers that belong to the specified branch
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="branchId">the branch that the search is limited to</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        public PagedResults<Customer> GetPagedCustomersForBranch(PagingModel paging, string branchId, string searchTerm, CustomerSearchType searchType) {
            var whereClause = string.Format("WHERE u_organization_type = '0' AND u_branch_number = '{0}'", branchId);

            return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
        }

        public PagedResults<Customer> GetPagedCustomersForBranches(PagingModel paging, List<string> branches, string searchTerm, CustomerSearchType searchType)
        {
            List<string> branchList = new List<string>();

            foreach (string branch in branches)
            {
                branchList.Add(string.Format("'{0}'", branch));
            }

            var whereClause = string.Format("WHERE u_organization_type = '0' AND u_branch_number IN ({0})", String.Join(",", branchList));

            return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
        }

        /// <summary>
        /// Search for customers that belong to the specified DSM
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="dsmNumber">the DSM that the search is limited to</param>
        /// <param name="branchId">the DSM's branch</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        public PagedResults<Customer> GetPagedCustomersForDSM(PagingModel paging, string dsmNumber, string branchId, string searchTerm, CustomerSearchType searchType) {
            var whereClause = string.Format("WHERE u_organization_type = '0' AND u_dsm_number = '{0}' AND u_branch_number = '{1}'", dsmNumber, branchId);

            return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
        }

        /// <summary>
        /// Search for customers that belong to the list of DSRs
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="dsrList">the DSRs that the search is limited to</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        public PagedResults<Customer> GetPagedCustomersForDSR(PagingModel paging, string searchTerm, List<Dsr> dsrList, CustomerSearchType searchType)
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
                returnValue = RetrievePagedResults(paging, searchTerm, whereText.ToString(), searchType);
            }
                        
            return returnValue;

		}

        /// <summary>
        /// search for customers that belong to the specified user
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="userId">the user that the search is limited to</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        public PagedResults<Customer> GetPagedCustomersForUser(PagingModel paging, Guid userId, string searchTerm, CustomerSearchType searchType)
		{
			var whereClause = string.Format("inner join [BEK_Commerce_profiles].[dbo].[UserOrganizationObject] uoo on oo.u_org_id = uoo.u_org_id WHERE uoo.u_user_id = '{0}' and u_organization_type = '0'", userId.ToCommerceServerFormat());

            return RetrievePagedResults(paging, searchTerm, whereClause, searchType);
		}

        private string GetTermKey(string branchId, string termCode) {
            StringBuilder output = new StringBuilder();

            output.Append(branchId.ToLower());
            output.Append("-");
            output.Append(termCode);

            return output.ToString();
    }

        public void RemoveUserFromCustomer(string removedBy, Guid customerId, Guid userId) {
            base.RemoveUserFromOrg(customerId, userId);
            _customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Format("user_{0}", userId.ToString())));
            _auditLogRepository.WriteToAuditLog(Common.Core.Enumerations.AuditType.UserRemovedFromCustomer, removedBy, string.Format("Customer: {0}, User: {1}", customerId, userId));
        }

        private Dictionary<string, Dsr> RetrieveDsrDictionary() {
            try {
                Dictionary<string, Dsr> cachedallDsrInfo = _customerCacheRepository.GetItem<Dictionary<string, Dsr>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("dsrInfo"));
                if (cachedallDsrInfo == null) {
                    var dsrInfo = _dsrLogic.GetAllDsrInfo();

                    if(dsrInfo == null || dsrInfo.Count == 0) {
                        _logger.WriteErrorLog("No DSRs returned from RetrieveDsrList in CustomerRepository, is BranchSupport.Dsrs populated?");

                        return new Dictionary<string, Dsr>();
                    }else {
                        Dictionary<string, Dsr> results = dsrInfo.ToDictionary(d => GetDsrKey(d.Branch, d.DsrNumber));

                        _customerCacheRepository.AddItem<Dictionary<string, Dsr>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("dsrInfo"), TimeSpan.FromHours(4), results);

                        return results;
                    }

                } else {
                    return cachedallDsrInfo;
                }
            } catch {
                throw;
            }
        }

        private Dictionary<string, string> RetrieveTermsCodeDict() {
            Dictionary<string, string> cachedDict = _customerCacheRepository.GetItem<Dictionary<string, string>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("termCodes"));
            if(cachedDict == null) {
                Dictionary<string, string> termDict = _termRepo.ReadAll().ToDictionary(t => GetTermKey(t.BranchId, t.TermCode.ToString()), t => t.Description.Trim());

                _customerCacheRepository.AddItem<Dictionary<string, string>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey("termsCodes"), TimeSpan.FromHours(4), termDict);

                return termDict;
            } else {
                return cachedDict;
            }
        }

        /// <summary>
        /// perform the seach for all of the paged customer methods
        /// </summary>
        /// <param name="paging">paging model</param>
        /// <param name="searchTerm">the terms to use during the search</param>
        /// <param name="whereClause">the where clause that was built in each of the GetPageCustomer methods</param>
        /// <param name="searchType">search on customer name/number, NA, or RA info</param>
        /// <returns>paged list of customers</returns>
        private PagedResults<Customer> RetrievePagedResults(PagingModel paging, string searchTerm, string whereClause, CustomerSearchType searchType) {
            var queryOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");

            queryOrg.SearchCriteria.WhereClause = BuildWhereClauseForCustomerSearch(whereClause, searchTerm, searchType);
            queryOrg.SearchCriteria.FirstItemIndex = paging.From.HasValue ? paging.From.Value : 0;
            queryOrg.SearchCriteria.NumberOfItemsToReturn = paging.Size.HasValue ? paging.Size.Value : int.MaxValue;
            queryOrg.SearchCriteria.ReturnTotalItemCount = true;

            if (paging.Sort != null && paging.Sort.Count > 0) {
                queryOrg.SearchCriteria.SortProperties = new List<CommerceSortProperty>();
                foreach (var sortOption in paging.Sort) {
                    switch (sortOption.Field.ToLower()) {
                        case "customername":
                            CommerceSortProperty sortName = new CommerceSortProperty() { 
                                CommerceEntityModelName = "u_name", 
                                SortDirection = sortOption.SortOrder == SortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending 
                            };
                            queryOrg.SearchCriteria.SortProperties.Add(sortName);
                            break;
                        case "customernumber":
                            CommerceSortProperty sortNumber = new CommerceSortProperty() {
                                CommerceEntityModelName = "u_customer_number",
                                SortDirection = sortOption.SortOrder == SortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending
                            };
                            queryOrg.SearchCriteria.SortProperties.Add(sortNumber);
                            break;
                    }
                }
            }

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(queryOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;
            List<Customer> results = BuildCustomerList(res.CommerceEntities);


            return new PagedResults<Customer>() { Results = results.ToList(), TotalResults = res.TotalItemCount ?? 0};
        }

        public void UpdateCustomerCanViewPricing(Guid customerId, bool canView) {
            CommerceUpdate<Organization> update = new CommerceUpdate<Organization>();
            // have to use the Commerce Server ID to search for customers so we cannot use branch/customer number
            update.SearchCriteria.Model.Id = customerId.ToCommerceServerFormat();
            update.Model.CanViewPricing = canView;
            
            CommerceUpdateOperationResponse res = FoundationService.ExecuteRequest(update.ToRequest()).OperationResponses[0] as CommerceUpdateOperationResponse;

            if(res.CommerceEntities.Count > 0){
                Organization org = new Organization(res.CommerceEntities[0]);

                _customerCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(string.Join("-", org.CustomerNumber, org.BranchNumber)));
            }
        }
		#endregion
	}
}
