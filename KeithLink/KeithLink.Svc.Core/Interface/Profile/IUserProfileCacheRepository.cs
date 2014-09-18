using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserProfileCacheRepository
    {
        UserProfile GetProfile(string emailAddress);
        void AddProfile(UserProfile userProfile);
        void ResetAllItems();
        void RemoveItem(string emailAddress);
    }
}
