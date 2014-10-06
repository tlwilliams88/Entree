using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.Profile;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class InternalUserDomainRepositoryTests {
        #region attributes
        private EventLogRepositoryImpl _log;
        private InternalUserDomainRepository _userRepo;
        #endregion

        #region ctor
        public InternalUserDomainRepositoryTests() {
            _log = new EventLogRepositoryImpl("Unit tests");
            _userRepo = new InternalUserDomainRepository(_log);
        }
        #endregion

        #region tests
        [TestMethod]
        public void CanAuthenticate() {
            bool success = _userRepo.AuthenticateUser("tcfox", "password");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup() {
            bool hasAccess = _userRepo.IsInGroup("lmloggins", Svc.Core.Constants.ROLE_INTERNAL_DSR_FDF);

            Assert.IsTrue(hasAccess);
        }
        #endregion
    }
}
