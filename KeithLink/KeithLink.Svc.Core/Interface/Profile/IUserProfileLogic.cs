using KeithLink.Svc.Core.Models.Profile;
using System;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        void CreateBekUserProfile(string emailAddress);

        UserProfileReturn CreateGuestUserAndProfile(string emailAddress, string password, string branchId);

        UserProfileReturn CreateUserAndProfile(string customerName, string emailAddress, string password, string firstName, string lastName, string phone, string roleName, string branchId);

        UserProfile FillUserProfile(Models.Generated.UserProfile csProfile);

        UserProfileReturn GetUserProfile(string emailAddress);

        bool IsInternalAddress(string emailAddress);

        string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);

        void UpdateUserProfile(Guid id, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId);

        AccountReturn CreateAccount(string name);

        CustomerReturn GetAllCustomers();
    }
}
