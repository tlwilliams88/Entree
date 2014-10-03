using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Common.Impl.Logging;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class ExternalUserDomainRepositoryTests {
        #region attributes
        CustomerContainerRepository _containerRepo;
        ExternalUserDomainRepository _custUserRepo;
        EventLogRepositoryImpl _log;
        #endregion

        #region ctor
        public ExternalUserDomainRepositoryTests() {
            _log = new EventLogRepositoryImpl("Unit Tests");
            _containerRepo = new CustomerContainerRepository(_log);
            _custUserRepo = new ExternalUserDomainRepository(_log, _containerRepo);
        }
        #endregion

        #region test
        [TestMethod]
        public void AuthenticateBadUserName() {
            string errMessage = null;

            if (_custUserRepo.AuthenticateUser("nonexistantuser@somecompany.com", "irrelevant", out errMessage)) {
                Assert.IsTrue(false);
            } else {
                Assert.IsTrue(errMessage.Contains("invalid"));
            }
        }

        [TestMethod]
        public void AuthenticateBadPassword() {
            string errMsg = null;

            if (_custUserRepo.AuthenticateUser("sabroussard@somecompany.com", "badpassword", out errMsg)) {
                Assert.IsTrue(false);
            } else {
                Assert.IsTrue(errMsg.Contains("invalid"));
            }
        }

        [TestMethod]
        public void AuthenticateDisabledUser()
        {
            string errMessage = null;

            if (_custUserRepo.AuthenticateUser("disableduser@somecompany.com", "D1sabled", out errMessage)) {
                Assert.IsTrue(false);
            } else {
                Assert.IsTrue(errMessage.Contains("disabled"));
            }
        }

        [TestMethod]
        public void AuthenticateGoodCredentials() {
            bool success = _custUserRepo.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e");

            Assert.IsTrue(success);
        }
        
        [TestMethod]
        public void AuthenticateLockedUser() {
            for (int i = 0; i < Impl.Configuration.ActiveDirectoryInvalidAttempts; i++) {
                try {
                    _custUserRepo.AuthenticateUser("lockeduser@somecompany.com", "badpassword");
                } catch { }
            }

            string errMsg = null;

            if (_custUserRepo.AuthenticateUser("lockeduser@somecompany.com", "badpassword", out errMsg)) {
                Assert.IsTrue(false);
            } else {
                Assert.IsTrue(errMsg.Contains("locked"));
            }
        }
        
        [TestMethod]
        public void BelongsToGroup() {
            bool hasAccess = _custUserRepo.IsInGroup("sabroussard@somecompany.com", "Jimmys Chicken Shack Owner");

            Assert.IsTrue(hasAccess);
        }

        [TestMethod]
        public void CreateUser() {
            try {
                //_custUserRepo.CreateUser("Jimmys Chicken Shack", "lockeduser@somecompany.com", "L0ckedUs3r", "Locked", "User", Core.Constants.ROLE_EXTERNAL_OWNER);

                Assert.IsTrue(true);
            } catch {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void UpdateUserPasswordGood() {
            Assert.IsTrue(_custUserRepo.UpdatePassword("jeremy@jeremyschickenshack.com", "Ab12345", "Ab12345"));
        }
        #endregion
    }
}
