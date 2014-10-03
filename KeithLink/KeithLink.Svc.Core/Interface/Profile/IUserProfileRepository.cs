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
        void CreateUserProfile(string emailAddress, string firstName, string lastName, string phoneNumber);

        void DeleteUserProfile(string userName);
        
        Core.Models.Generated.UserProfile GetCSProfile(string emailAddress);
        
        void UpdateUserProfile(Guid id, string emailAddres, string firstName, string lastName, string phoneNumber, string branchId);
    }
}
