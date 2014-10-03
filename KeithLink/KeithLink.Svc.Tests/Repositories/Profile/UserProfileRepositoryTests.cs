﻿using KeithLink.Svc.Core.Models.Profile;
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
                    new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository());

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
                    new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository());

            //userProfile.CreateGuestProfile("jeremy@ames.com", "Ab12345", "FDF");
        }

        [TestMethod]
        public void UpdateUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository userProfile =
                new Impl.Repository.Profile.UserProfileRepository(
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"),
                    new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts")),
                    new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository());

            //UserProfileReturn userReturn = userProfile.GetUserProfile("jeremy@jeremyschickenshack.com");

            //userProfile.UpdateUserProfile(userReturn.UserProfiles[0].UserId, 
            //                               "jeremy@jeremyschickenshack.com",
            //                               "Jeremy",
            //                               "Ames",
            //                               "817-877-5700",
            //                               "FDF");
        }
    }
}
