using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class UserProfileRepositoryTests
    {
        [TestMethod]
        public void CreateUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")));

            //userProfile.CreateUserProfile("mytest", "Test Customer", "joesmith@company.com", "Joe", "Smith", "1234567890");
			//userProfile.CreateUserProfile("Jimmys Chicken Shack", "sabroussard@somecompany.com", "L1ttleStev1e", "Steven", "Broussard", "(817)877-5700", "Owner");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CreateGuest() {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")));

            userProfile.CreateGuestProfile("one@two.com", "Ab12345", "FDF");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")));
            userProfile.GetUserProfile("sabroussard@somecompany.com");
        }

        [TestMethod]
        public void GetBEKUserProfile()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")));
            userProfile.GetUserProfile("jwames@benekeith.com");
        }

        [TestMethod]
        public void AuthenticateUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")));

            Assert.IsTrue(userProfile.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e"));
        }

        //[TestMethod]
        //public void AuthenticateUserAndRetrieveUserProfile()
        //{
        //    KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository repo = new Impl.Repository.Profile.UserProfileRepository();
        //    KeithLink.Svc.Core.Models.Profile.UserProfileReturn profileReturn = null;

        //    repo.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e", out profileReturn);

        //    Assert.IsNotNull(profileReturn);
        //}
    }
}
