using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Repository.Profile.Cache
{
    public class NoCacheUserProfileCacheRepository : IUserProfileCacheRepository
    {
        public Core.Models.Profile.UserProfile GetProfile(string emailAddress)
        {
            return null; // do nothing
        }

        public void AddProfile(Core.Models.Profile.UserProfile userProfile)
        {
            // do nothing
        }

        public void ResetAllItems()
        {
            // do nothing
        }

        public void RemoveItem(string emailAddress)
        {
            // do nothing
        }
    }
}
