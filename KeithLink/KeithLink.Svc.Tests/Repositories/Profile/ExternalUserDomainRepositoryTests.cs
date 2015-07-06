using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Authentication;
using KeithLink.Svc.Core.Enumerations.Authentication;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.Profile;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using KeithLink.Common.Impl.AuditLog;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class ExternalUserDomainRepositoryTests {
        #region attributes
        ExternalUserDomainRepository _custUserRepo;
        EventLogRepositoryImpl _log;
        #endregion

        #region ctor
        public ExternalUserDomainRepositoryTests() {
            _log = new EventLogRepositoryImpl("Unit Tests");
			var _auditLog = new AuditLogRepositoryImpl();
            _custUserRepo = new ExternalUserDomainRepository(_log, _auditLog);
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
                //_custUserRepo.CreateUser("Jimmys Chicken Shack", "lockeduser@somecompany.com", "L0ckedUs3r", "Locked", "User", Configuration.RoleNameOwner);

                Assert.IsTrue(true);
            } catch {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void GoodUserWithKbitAccess() {
            Assert.IsTrue(_custUserRepo.HasAccess("sabroussard@somecompany.com", "Dev Kbit Customer"));
        }

        //[TestMethod]
        //public void SucessAddUserToGroup() {
        //    _custUserRepo.GrantAccess("sabroussard@somecompany.com", "Dev Kbit Customer");
        //}

        //[TestMethod]
        //public void SuccessRemoveUserFromGroup() {
        //    _custUserRepo.RevokeAccess("sabroussard@somecompany.com", "Dev Kbit Customer");
        //}

        [TestMethod]
        public void UpdateUserPasswordGood() {
			Assert.IsTrue(_custUserRepo.UpdatePassword(TestSessionObject.TestAuthenticatedUser.EmailAddress, "lockeduser@somecompany.com", "Ab12345", "Ab12345"));
        }

        //[TestMethod]
        //public void MyTestMethod()
        //{
        //    _custUserRepo.UpdateUserGroups(new List<string>(), "owner", "sabroussard@somecompany.com") ;
        //}

        [TestMethod]
        public void DeleteUser() {
            const string USER_EMAIL = "deletableuser@somecompany.com";

            _custUserRepo.CreateUser("Jimmys Chicken Shack", USER_EMAIL , "Ab12345", "First", "Last", Configuration.RoleNameOwner);

            _custUserRepo.DeleteUser(USER_EMAIL);
        }
        #endregion
    }
}
