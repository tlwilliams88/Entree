using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserProfileRepository
    {
        //bool AuthenticateUser(string emailAddress, string password);
        //bool AuthenticateUser(string emailAddress, string password, out string errorMessage);
        UserProfileReturn CreateUserProfile(string customerName, string emailAddres, string password, string firstName, string lastName, string phoneNumber, string roleName);
        UserProfileReturn CreateGuestProfile(string emailAddres, string password, string branchId);
        void DeleteUserProfile(string userName);
        UserProfileReturn GetUserProfile(string userName);
        UserProfileReturn GetUserProfilesByCustomerName(string customerName);
        string UpdateUserPassword(string emailAddress, string originalPassword, string newPassword);
        void UpdateUserProfile(Guid id, string emailAddres, string firstName, string lastName, string phoneNumber, string branchId);
    }
}
