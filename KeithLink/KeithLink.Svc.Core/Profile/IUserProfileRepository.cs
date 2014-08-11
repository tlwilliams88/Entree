using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    public interface IUserProfileRepository
    {
        bool AuthenticateUser(string emailAddress, string password);
        bool AuthenticateUser(string emailAddress, string password, out Core.Profile.UserProfileReturn userProfile);
        void CreateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber);
        void DeleteUserProfile(string userName);
        UserProfileReturn GetUserProfile(string userName);
        UserProfileReturn GetUserProfilesByCustomerName(string customerName);
        void UpdateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber);
    }
}
