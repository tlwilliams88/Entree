using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_UserProfileRepository
    {
        [TestMethod]
        public void CreateUser()
        {
            KeithLink.Svc.Impl.Profile.UserProfileRepository userProfile = new Impl.Profile.UserProfileRepository();

            //userProfile.CreateUserProfile("mytest", "Test Customer", "joesmith@company.com", "Joe", "Smith", "1234567890");
            userProfile.CreateUserProfile("sabroussard@somecompany.com", "Jimmys Chicken Shack", "sabroussard@somecompany.com", "Steven", "Broussard", "1234567890");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
            KeithLink.Svc.Impl.Profile.UserProfileRepository userProfile = new Impl.Profile.UserProfileRepository();

            userProfile.GetUserProfile("sabroussard@somecompany.com");
        }

        [TestMethod]
        public void AuthenticateUser()
        {
            KeithLink.Svc.Impl.Profile.UserProfileRepository userProfile = new Impl.Profile.UserProfileRepository();

            Assert.IsTrue(userProfile.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e"));
        }

        [TestMethod]
        public void AuthenticateUserAndRetrieveUserProfile()
        {
            KeithLink.Svc.Impl.Profile.UserProfileRepository repo = new Impl.Profile.UserProfileRepository();
            KeithLink.Svc.Core.Profile.UserProfileReturn profileReturn = null;

            repo.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e", out profileReturn);

            Assert.IsNotNull(profileReturn);
        }
    }
}
