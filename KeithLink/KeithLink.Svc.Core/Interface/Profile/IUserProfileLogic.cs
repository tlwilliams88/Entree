using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
		void AddUserToCustomer(UserProfile addedBy, Guid customerId, Guid userId);

		AccountReturn CreateAccount(UserProfile createdBy, string name);

		void DeleteAccount(UserProfile deletedBy, Guid accountId);

        void CreateBekUserProfile(string emailAddress);

        DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr);
        
        UserProfileReturn CreateGuestUserAndProfile(UserProfile actiingUser, string emailAddress, string password, string branchId);

		UserProfileReturn CreateUserAndProfile(UserProfile actiingUser, string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId);

        PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, PagingModel paging, string account, CustomerSearchType searchType);

        void DeleteDsrAlias(long dsrAliasId, string email);

        //UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);
        UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false);

        Account GetAccount(UserProfile user, Guid id);
        
        AccountReturn GetAccounts(AccountFilterModel accountFilters);
        
        AccountUsersReturn GetAccountUsers(Guid id);

        List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId);

        CustomerBalanceOrderUpdatedModel GetBalanceForCustomer(string customerId, string branchId);

        //CustomerReturn GetCustomers(CustomerFilterModel customerFilters);
        
        Customer GetCustomerByCustomerNumber(string customerNumber, string branchId);

        List<Customer> GetCustomersForExternalUser(Guid userId);

        Customer GetCustomerForUser(string customerNumber, string branchId, Guid userId);

        List<Models.Messaging.ProfileMessagingPreferenceModel> GetMessagingPreferences(Guid guid);
		List<ProfileMessagingPreferenceModel> GetMessagingPreferencesForCustomer(Guid guid, string customerId, string branchId);

        List<Customer> GetNonPagedCustomersForUser(UserProfile user, string search = "");

        PagedResults<Account> GetPagedAccounts(PagingModel paging);

        UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false);
        
        UserProfileReturn GetUserProfile(Guid userId, bool includeLastOrderDate = true);

        UserProfileReturn GetUsers(UserFilterModel userFilters);


		void GrantRoleAccess(UserProfile updatedBy, string emailAddress, AccessRequestType requestedApp);

        bool IsInternalAddress(string emailAddress);

        void RemoveUserFromAccount(UserProfile removedBy, Guid accountId, Guid userId);

        void RemoveUserFromCustomer(UserProfile removedBy, Guid customerId, Guid userId);

        void ResetPassword(Guid userId, string newPassword);

		void RevokeRoleAccess(UserProfile updatedBy, string emailAddress, AccessRequestType requestedApp);

		bool UpdateAccount(UserProfile updatedBy, Guid accountId, string name, List<Customer> customers, List<UserProfile> users);

		bool UpdateUserPassword(UserProfile updatedBy, string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(UserProfile updatedBy, Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId, bool updateCustomerListAndRole, List<Customer> customerList, string roleName);

        //void UpdateUserRoles(List<string> customerNames, string emailAddress, string roleName);

		UserProfileReturn UserCreatedGuestWithTemporaryPassword(UserProfile actiingUser, string emailAddress, string branchId);

		List<UserProfile> GetInternalUsersWithAccessToCustomer(string customerNumber, string branchId);

        List<SettingsModel> GetProfileSettings( Guid userId );

        void SaveProfileSettings( SettingsModel settings );
    }
}
