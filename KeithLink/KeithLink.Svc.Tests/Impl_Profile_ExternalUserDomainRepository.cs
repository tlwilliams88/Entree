using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_ExternalUserDomainRepository
    {
        [TestMethod]
        public void CanAuthenticate()
        {
            KeithLink.Svc.Impl.Profile.ExternalUserDomainRepository ad = new Impl.Profile.ExternalUserDomainRepository();
            bool success = ad.AuthenticateUser("sabroussard@benekeith.com", "L1ttleStev1e");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup()
        {
            KeithLink.Svc.Impl.Profile.ExternalUserDomainRepository ad = new Impl.Profile.ExternalUserDomainRepository();
            bool hasAccess = ad.IsInGroup("sabroussard@benekeith.com", "Owner");

            Assert.IsTrue(hasAccess);
        }
    }
}
