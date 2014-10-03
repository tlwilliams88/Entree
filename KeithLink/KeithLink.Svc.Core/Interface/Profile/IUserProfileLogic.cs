using KeithLink.Svc.Core.Models.Profile;
using System;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        void AssertCustomerNameLength(string customerName);
        void AssertCustomerNameValidCharacters(string customerName);
        void AssertEmailAddress(string emailAddress);
        void AssertEmailAddressLength(string emailAddress);
        void AssertEmailAddressUnique(string emailAddress);
        void AssertFirstNameLength(string firstName);
        void AssertGuestProfile(string emailAddress, string password);
        void AssertLastNameLength(string lastName);
        void AssertPasswordComplexity(string password);
        void AssertPasswordLength(string password);
        void AssertPasswordVsAttributes(string password, string firstName, string lastName);
        void AssertRoleName(string roleName);
        void AssertRoleNameLength(string roleName);
        void AssertUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName);
                
        void CreateBekUserProfile(string emailAddress);

        UserProfileReturn CreateGuestUserAndProfileProfile(string emailAddress, string password, string branchId);

        UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName);

        UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);

        UserProfileReturn GetUserProfile(string emailAddress);

        bool IsInternalAddress(string emailAddress);

        string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId);
    }
}
