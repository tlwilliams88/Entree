﻿using KeithLink.Svc.Core.Models.Paging;
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
        List<Customer> GetCustomers();
		PagedResults<Customer> GetPagedCustomers(int size, int from, string searchTerm);
		Customer GetCustomerById(Guid customerId);
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);
        void AddUserToCustomer(Guid customerId, Guid userId);
        void RemoveUserFromCustomer(Guid customerId, Guid userId);
        List<Core.Models.Profile.Customer> GetCustomersForUser(Guid userId);
        List<Customer> GetCustomersByNameSearch(string searchText);
        List<Customer> GetCustomersByNameSearchAndBranch(string search, string branchId);
		List<Customer> GetCustomersForDSR(string dsrNumber, string branchId);
        List<Customer> GetCustomersForDSM(string dsmNumber, string branchId);
		List<Customer> GetCustomersForAccount(string accountId);
		List<Customer> GetCustomersByNameOrNumber(string search);
		List<Customer> GetCustomersForParentAccountOrganization(string accountId);
        void ClearCustomerCache();
    }
}
