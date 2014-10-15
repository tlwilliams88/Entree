using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class UserProfileRepositoryTests {
        #region attributes
        private NoCacheUserProfileCacheRepository _cache;
        private EventLogRepositoryImpl _log;
        private UserProfileRepository _profile;
        #endregion

        #region ctor
        public UserProfileRepositoryTests() {
            _log = new EventLogRepositoryImpl("Unit tests");
            _cache = new NoCacheUserProfileCacheRepository();
            _profile = new UserProfileRepository(_log, _cache);
        }
        #endregion

        #region tests
        [TestMethod]
        public void CreateUser() {
            //_profile.CreateUserProfile("joesmith@company.com", "Joe", "Smith", "1234567890");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UpdateUser() {
            KeithLink.Svc.Core.Models.Generated.UserProfile userProfile = _profile.GetCSProfile("jeremy@jeremyschickenshack.com");

            _profile.UpdateUserProfile(new System.Guid(userProfile.Id), "jeremy@jeremyschickenshack.com", "Jeremy", "Ames", "817-877-5700", "FDF");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
            KeithLink.Svc.Core.Models.Generated.UserProfile userProfile = _profile.GetCSProfile("jeremy@jeremyschickenshack.com");

            Assert.IsTrue(userProfile != null);
        }
        #endregion
    }
}
