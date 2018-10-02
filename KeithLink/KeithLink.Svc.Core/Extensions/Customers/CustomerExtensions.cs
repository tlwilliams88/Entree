using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Customers
{
    public static class CustomerExtensions
    {
        public static PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel> ToPagedForCustomersIntegration(this PagedResults<Customer> pagedCustomers) {
            PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel> returnValue = new PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel>();
            returnValue.Results = new List<CustomerIntegrationsReturnModel>();
            foreach (Customer model in pagedCustomers.Results) {
                CustomerIntegrationsReturnModel returnCustomer = new CustomerIntegrationsReturnModel();
                returnCustomer.CustomerName = model.CustomerName;
                returnCustomer.CustomerNumber = model.CustomerNumber;
                returnCustomer.BranchId = model.CustomerBranch;

                returnValue.Results.Add(returnCustomer);
            }

            return returnValue;
        }
    }
}
