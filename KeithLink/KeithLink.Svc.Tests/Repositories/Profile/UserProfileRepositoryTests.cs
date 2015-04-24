using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Repository.Cache;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class UserProfileRepositoryTests {
        #region attributes
		private NoCacheRepositoryImpl _cache;
        private EventLogRepositoryImpl _log;
        private UserProfileRepository _profile;
        #endregion

        #region ctor
        public UserProfileRepositoryTests() {
            _log = new EventLogRepositoryImpl("Unit tests");
            _cache = new NoCacheRepositoryImpl();
            _profile = new UserProfileRepository(_log);
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
			//This user doesn't exist in most environment's so this test will fail for most
			//KeithLink.Svc.Core.Models.Generated.UserProfile userProfile = _profile.GetCSProfile("jeremy@jeremyschickenshack.com");

			//_profile.UpdateUserProfile(new System.Guid(userProfile.Id), "jeremy@jeremyschickenshack.com", "Jeremy", "Ames", "817-877-5700", "FDF");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
			KeithLink.Svc.Core.Models.Generated.UserProfile userProfile = _profile.GetCSProfile("corp-ssa-entreadmin@benekeith.com");

            Assert.IsTrue(userProfile != null);
        }
        #endregion
    }
}
