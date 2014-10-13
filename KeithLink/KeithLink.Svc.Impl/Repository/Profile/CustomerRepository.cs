using CommerceServer.Foundation;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class CustomerRepository : Core.Interface.Profile.ICustomerRepository
    {
        #region attributes
        IEventLogRepository _logger;
        IUserProfileCacheRepository _userProfileCacheRepository;
        #endregion

        #region ctor
        public CustomerRepository(IEventLogRepository logger, IUserProfileCacheRepository userProfileCacheRepository)
        {
            _logger = logger;
            _userProfileCacheRepository = userProfileCacheRepository;
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

            var createOrg = new CommerceServer.Foundation.CommerceQuery<KeithLink.Svc.Core.Models.Generated.Organization>("Organization");
            createOrg.SearchCriteria.Model.OrganizationType = "0"; // org type of customer

            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            var customers = new System.Collections.Concurrent.BlockingCollection<Customer>();
            System.Threading.Tasks.Parallel.ForEach(res.CommerceEntities, e =>
                {
                    KeithLink.Svc.Core.Models.Generated.Organization org = new KeithLink.Svc.Core.Models.Generated.Organization(e);
                    customers.Add(new Customer()
                    {
                        CustomerId = org.Id,
                        AccountId = "",
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

            return  customers.ToList();
        }


        public void AddUserToCustomer(Guid customerId, Guid userId, string role)
        {
        }

        public void RemoveUserFromCustomer(Guid customerId, Guid userId)
        {
        }


        #endregion
    }
}
