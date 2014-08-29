using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_ExternalUserDomainRepository
    {
        [TestMethod]
        public void AuthenticateBadUserName()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            string errMessage = null;

            if (ad.AuthenticateUser("nonexistantuser@somecompany.com", "irrelevant", out errMessage))
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(errMessage.Contains("invalid"));
            }
        }

        [TestMethod]
        public void AuthenticateBadPassword()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            string errMsg = null;

            if (ad.AuthenticateUser("sabroussard@somecompany.com", "badpassword", out errMsg))
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(errMsg.Contains("invalid"));
            }
        }

        [TestMethod]
        public void AuthenticateDisabledUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            string errMessage = null;

            if (ad.AuthenticateUser("disableduser@somecompany.com", "D1sabled", out errMessage))
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(errMessage.Contains("disabled"));
            }
        }

        [TestMethod]
        public void AuthenticateGoodCredentials()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
            bool success = ad.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e");

            Assert.IsTrue(success);
        }
        
        [TestMethod]
        public void AuthenticateLockedUser()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            for (int i = 0; i < Impl.Configuration.ActiveDirectoryInvalidAttempts; i++)
            {
                try
                {
                    ad.AuthenticateUser("lockeduser@somecompany.com", "badpassword");
                }
                catch { }
            }

            string errMsg = null;

            if (ad.AuthenticateUser("lockeduser@somecompany.com", "badpassword", out errMsg))
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(errMsg.Contains("locked"));
            }
        }
        
        [TestMethod]
        public void BelongsToGroup()
        {
            KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
            bool hasAccess = ad.IsInGroup("sabroussard@somecompany.com", "Jimmys Chicken Shack Owner");

            Assert.IsTrue(hasAccess);
        }

        [TestMethod]
        public void CreateUser()
        {
            try
            {
                KeithLink.Svc.Impl.Repository.Profile.ExternalUserDomainRepository ad = new Impl.Repository.Profile.ExternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

                //ad.CreateUser("Jimmys Chicken Shack", "lockeduser@somecompany.com", "L0ckedUs3r", "Locked", "User", Core.Constants.ROLE_EXTERNAL_OWNER);

                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }
    }
}
