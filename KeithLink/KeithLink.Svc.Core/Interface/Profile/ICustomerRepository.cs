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
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);
        void AddUserToCustomer(Guid customerId, Guid userId);
        void RemoveUserFromCustomer(Guid customerId, Guid userId);
        List<Core.Models.Profile.Customer> GetCustomersForUser(Guid userId);
        List<Customer> GetCustomersByNameSearch(string searchText);
		List<Customer> GetCustomersForDSR(string dsrNumber);
		List<Customer> GetCustomersForAccount(string accountId);
		List<Customer> GetCustomersByNameOrNumber(string search);
		List<Customer> GetCustomersForParentAccountOrganization(string accountId);
        void ClearCustomerCache();
    }
}
