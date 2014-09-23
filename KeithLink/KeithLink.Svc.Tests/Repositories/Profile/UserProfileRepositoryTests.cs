using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
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
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());

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
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());

            //userProfile.CreateGuestProfile("one@two.com", "Ab12345", "FDF");
        }

        [TestMethod]
        public void GetUserByEmailAddress()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());
            userProfile.GetUserProfile("sabroussard@somecompany.com");
        }

        [TestMethod]
        public void GetBEKUserProfile()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());
            userProfile.GetUserProfile("jwames@benekeith.com");
        }

        [TestMethod]
        public void AuthenticateUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());

            Assert.IsTrue(userProfile.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e"));
        }

        [TestMethod]
        public void UpdateUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.NoCacheUserProfileCacheRepository());
            
             UserProfileReturn userReturn = userProfile.GetUserProfile("sabroussard@somecompany.com");

             userProfile.UpdateUserProfile(userReturn.UserProfiles[0].UserId, 
                                           userReturn.UserProfiles[0].EmailAddress,
                                           "John",
                                           "Lennon",
                                           "817-877-5700",
                                           "FDF");
        }
    }
}
