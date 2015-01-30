using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        void CreateBekUserProfile(string emailAddress);

        UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId);
        UserProfileReturn UserCreatedGuestWithTemporaryPassword( string emailAddress, string branchId );

        UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId);
        void UpdateUserRoles(List<string> customerNames, string emailAddress, string roleName);

        //UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);
		UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false);

        UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false);
        UserProfileReturn GetUserProfile(Guid userId, bool includeLastOrderDate = true);

        bool IsInternalAddress(string emailAddress);

        bool UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId, bool updateCustomerListAndRole, List<Customer> customerList, string roleName);

		PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, PagingModel paging, string account);
        List<Models.Messaging.ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid);

        // admin functions
        CustomerReturn GetCustomers(CustomerFilterModel customerFilters);
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);
        AccountReturn GetAccounts(AccountFilterModel accountFilters);
        Account GetAccount(Guid id);
        AccountUsersReturn GetAccountUsers(Guid id);
        UserProfileReturn GetUsers(UserFilterModel userFilters);
        AccountReturn CreateAccount(string name);
        bool UpdateAccount(Guid accountId, string name, List<Customer> customers, List<UserProfile> users);
        void AddCustomerToAccount(Guid accountId, Guid customerId);
        void AddUserToCustomer(Guid customerId, Guid userId);
        void RemoveUserFromCustomer(Guid customerId, Guid userId);
		List<Customer> GetCustomersForUser(UserProfile user, string search = "");
        List<Customer> GetCustomersForExternalUser(Guid userId);
    }
}
