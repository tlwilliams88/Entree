using Autofac;
using KeithLink.Svc.Core.Interface.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic {
    [TestClass]
    public class UserProfileLogicImplTests {
        #region attrbitues
        private IUserProfileLogic _logic;
        #endregion

        #region ctor
        public UserProfileLogicImplTests() {
            var container = DependencyMap.Build();

            _logic = container.Resolve<IUserProfileLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetUserProfile() {
            Core.Models.Profile.UserProfileReturn userProfiles = _logic.GetUserProfile("test52@thismat.com");

            Assert.IsTrue(userProfiles.UserProfiles.Count == 1);
        }

        [TestMethod]
        public void SuccessfullyCreateGuest() {
            //_logic.CreateGuestUserAndProfile("testguest@somecompany.com", "Ab12345", "FDF");
        }

        [TestMethod]
        public void SuccessfullyCreateGuestWithTempPassword() {
            //_logic.UserCreatedGuestWithTemporaryPassword("usercreatedguest@somecompany.com", "FDF");
        }

        public void SuccssfulCreateUserAndProfile() {
            //_logic.CreateUserAndProfile("Jeremys Chicken Shack", "test@somecompany.com", "Ab12345", "Test", "User", "", "accounting", "FDF");
        }

        [TestMethod]
        public void SuccessfullyGrantAccessToKbitCustomer() {
            _logic.GrantRoleAccess(TestSessionObject.TestAuthenticatedUser, "sabroussard@somecompany.com", Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer);
        }

        [TestMethod]
        public void SuccessfullyRemoveAccessToKbitCustomer() {
			_logic.RevokeRoleAccess(TestSessionObject.TestAuthenticatedUser, "sabroussard@somecompany.com", Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer);
        }
        #endregion
    }
}
