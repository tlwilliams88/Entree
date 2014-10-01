using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IUserProfileLogic {
        //void AssertCustomerNameLength(string customerName);
        //void AssertCustomerNameValidCharacters(string customerName);
        //void AssertEmailAddress(string emailAddress);
        //void AssertEmailAddressLength(string emailAddress);
        //void AssertEmailAddressUnique(string emailAddress);
        //void AssertFirstNameLength(string firstName);
        //void AssertGuestProfile(string emailAddress, string password);
        //void AssertLastNameLength(string lastName);
        //void AssertPasswordComplexity(string password);
        //void AssertPasswordLength(string password);
        //void AssertPasswordVsAttributes(string password, string firstName, string lastName);
        //void AssertRoleName(string roleName);
        //void AssertRoleNameLength(string roleName);
        //void AssertUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName);

        
        UserProfile CombineProfileFromCSAndAD(Core.Models.Generated.UserProfile csProfile);

        bool IsInternalAddress(string emailAddress);
    }
}
