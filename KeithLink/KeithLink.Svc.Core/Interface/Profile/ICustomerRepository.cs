﻿using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface ICustomerRepository
    {
        void AddUserToCustomer(string addedBy, Guid customerId, Guid userId);
        void ClearCustomerCache();
		Customer GetCustomerById(Guid customerId);
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);
		Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId);
        List<Customer> GetCustomers();
		List<Customer> GetCustomersByNameOrNumber(string search);
        List<Customer> GetCustomersByNameSearch(string searchText);
        List<Customer> GetCustomersByNameSearchAndBranch(string search, string branchId);
		List<Customer> GetCustomersForAccount(string accountId);
        List<Customer> GetCustomersForDSM(string dsmNumber, string branchId);
        List<Customer> GetCustomersForDSR(List<Dsr> dsrs);
		List<Customer> GetCustomersForParentAccountOrganization(string accountId);
        List<Customer> GetCustomersForUser(Guid userId);
        PagedResults<Customer> GetPagedCustomers(PagingModel paging, string searchTerm, CustomerSearchType searchType);
        PagedResults<Customer> GetPagedCustomersForAccount(PagingModel paging, string searchTerm, string accountId, CustomerSearchType searchType);
        PagedResults<Customer> GetPagedCustomersForBranch(PagingModel paging, string branchId, string searchTerm, CustomerSearchType searchType);
        PagedResults<Customer> GetPagedCustomersForDSM(PagingModel paging, string dsrNumber, string branchId, string searchTerm, CustomerSearchType searchType);
        PagedResults<Customer> GetPagedCustomersForDSR(PagingModel paging, string searchTerm, List<Dsr> dsrList, CustomerSearchType searchType);
        PagedResults<Customer> GetPagedCustomersForUser(PagingModel paging, Guid userId, string searchTerm, CustomerSearchType searchType);
        void RemoveUserFromCustomer(string removedBy, Guid customerId, Guid userId);
        void UpdateCustomerCanViewPricing(Guid customerId, bool canView);
    }
}
