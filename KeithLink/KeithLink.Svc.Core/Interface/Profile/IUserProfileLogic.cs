using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        void CreateBekUserProfile(string emailAddress);

        UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId, bool allowPasswordGeneration = false);

        UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId);

        //UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);
		UserProfile FillUserProfile(Core.Models.Generated.UserProfile csProfile, bool includeLastOrderDate = true, bool includeTermInformation = false);

        UserProfileReturn GetUserProfile(string emailAddress, bool includeTermInformation = false);
        UserProfileReturn GetUserProfile(Guid userId, bool includeLastOrderDate = true);

        bool IsInternalAddress(string emailAddress);

        string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId);

		PagedResults<Customer> CustomerSearch(UserProfile user, string searchTerms, PagingModel paging);


        // admin functions
        CustomerReturn GetCustomers(CustomerFilterModel customerFilters);
        AccountReturn GetAccounts(AccountFilterModel accountFilters);
        AccountReturn GetAccount(Guid id);
        UserProfileReturn GetUsers(UserFilterModel userFilters);
        AccountReturn CreateAccount(string name);
        bool UpdateAccount(Guid accountId, string name, List<Customer> customers, List<UserProfile> users);
        void AddCustomerToAccount(Guid accountId, Guid customerId);
        void AddUserToCustomer(Guid customerId, Guid userId);
        void RemoveUserFromCustomer(Guid customerId, Guid userId);
		List<Customer> GetCustomersForUser(UserProfile user);
    }
}
