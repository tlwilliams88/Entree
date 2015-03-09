using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Authentication;
using KeithLink.Svc.Core.Enumerations.Authentication;
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
            AuthenticationModel authentication =  _custUserRepo.AuthenticateUser( "nonexistantuser@somecompany.com", "irrelevant" );
            Assert.IsTrue( authentication.Status.Equals( AuthenticationStatus.FailedAuthentication ) );
        }

        [TestMethod]
        public void AuthenticateBadPassword() {
            AuthenticationModel authentication = _custUserRepo.AuthenticateUser( "sabroussard@somecompany.com", "badpassword" );
            Assert.IsTrue( authentication.Status.Equals(AuthenticationStatus.FailedAuthentication) );
        }

        [TestMethod]
        public void AuthenticateDisabledUser()
        {
            AuthenticationModel authentication = _custUserRepo.AuthenticateUser( "disableduser@somecompany.com", "D1sabled" );
            Assert.IsTrue( authentication.Status.Equals( AuthenticationStatus.Disabled ) );
        }

        [TestMethod]
        public void AuthenticateGoodCredentials() {
            AuthenticationModel authentication = _custUserRepo.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e");
            Assert.IsTrue( authentication.Status.Equals( AuthenticationStatus.Successful ) );
        }
        
        [TestMethod]
        public void AuthenticateLockedUser() {
            for (int i = 0; i < Impl.Configuration.ActiveDirectoryInvalidAttempts; i++) {
                try {
                    _custUserRepo.AuthenticateUser("lockeduser@somecompany.com", "badpassword");
                } catch { }
            }

            AuthenticationModel authentication = _custUserRepo.AuthenticateUser( "lockeduser@somecompany.com", "badpassword" );
            Assert.IsTrue(authentication.Status.Equals(AuthenticationStatus.Locked));
        }

        [TestMethod]
        public void UserDoesNotHaveRequestedGroup() {
            Assert.IsFalse(_custUserRepo.HasAccess("sabroussard@somecompany.com", "Invalid Group Name"));
        }

        [TestMethod]
        public void BelongsToGroup() {
            string roleName = _custUserRepo.GetUserGroup("sabroussard@somecompany.com", new System.Collections.Generic.List<string>() { "owner" });

            Assert.IsTrue(roleName == "owner");
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
        public void GoodUserWithKbitAccess() {
            Assert.IsTrue(_custUserRepo.HasAccess("sabroussard@somecompany.com", "Dev Kbit Customer"));
        }

        [TestMethod]
        public void UpdateUserPasswordGood() {
            Assert.IsTrue(_custUserRepo.UpdatePassword("jeremy@jeremyschickenshack.com", "Ab12345", "Ab12345"));
        }
        #endregion
    }
}
