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

            userProfile.CreateUserProfile("mytest", "Test Customer", "joesmith@company.com", "Joe", "Smith", "1234567890");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
            KeithLink.Svc.Impl.Profile.UserProfileRepository userProfile = new Impl.Profile.UserProfileRepository();

            userProfile.GetUserProfile("joesmith@company.com");
        }
    }
}
