using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        void AddCustomerToAccount(Guid accountId, Guid customerId);
        
        void AddUserToCustomer(Guid customerId, Guid userId);
        
        AccountReturn CreateAccount(string name);

        void CreateBekUserProfile(string emailAddress);

        UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId);
		
        UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId);

        PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, PagingModel paging, string account);

        //UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);
        UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false);

        Account GetAccount(Guid id);
        
        AccountReturn GetAccounts(AccountFilterModel accountFilters);
        
        AccountUsersReturn GetAccountUsers(Guid id);

        CustomerBalanceOrderUpdatedModel GetBalanceForCustomer(string customerId, string branchId);

        //CustomerReturn GetCustomers(CustomerFilterModel customerFilters);
        
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);

        List<Customer> GetCustomersForExternalUser(Guid userId);

        Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId);

        List<Models.Messaging.ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid);

        List<Customer> GetNonPagedCustomersForUser(UserProfile user, string search = "");

        PagedResults<Account> GetPagedAccounts(PagingModel paging);

        UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false);
        
        UserProfileReturn GetUserProfile(Guid userId, bool includeLastOrderDate = true);

        UserProfileReturn GetUsers(UserFilterModel userFilters);

        void GrantRoleAccess(string emailAddress, AccessRequestType requestedApp);

        bool IsInternalAddress(string emailAddress);

        void RemoveUserFromAccount(Guid accountId, Guid userId);

        void RemoveUserFromCustomer(Guid customerId, Guid userId);

        void ResetPassword(string emailAddress);

        void RevokeRoleAccess(string emailAddress, AccessRequestType requestedApp);

        bool UpdateAccount(Guid accountId, string name, List<Customer> customers, List<UserProfile> users);

        bool UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId, bool updateCustomerListAndRole, List<Customer> customerList, string roleName);

        void UpdateUserRoles(List<string> customerNames, string emailAddress, string roleName);
        
        UserProfileReturn UserCreatedGuestWithTemporaryPassword( string emailAddress, string branchId );
    }
}
