using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_InternalUserDomainRepository
    {
        [TestMethod]
        public void CanAuthenticate()
        {
            KeithLink.Svc.Impl.Profile.InternalUserDomainRepository ad = new Impl.Profile.InternalUserDomainRepository();
            bool success = ad.AuthenticateUser("tcfox", "password");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup()
        {
            KeithLink.Svc.Impl.Profile.InternalUserDomainRepository ad = new Impl.Profile.InternalUserDomainRepository();
            bool hasAccess = ad.IsInGroup("lmloggins", Svc.Core.Constants.ROLE_INTERNAL_DSR_FDF);

            Assert.IsTrue(hasAccess);
        }
    }
}
