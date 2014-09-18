using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class InternalUserDomainRepositoryTests
    {
        [TestMethod]
        public void CanAuthenticate()
        {
            KeithLink.Svc.Impl.Repository.Profile.InternalUserDomainRepository ad = new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
            bool success = ad.AuthenticateUser("tcfox", "password");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup()
        {
            KeithLink.Svc.Impl.Repository.Profile.InternalUserDomainRepository ad = new Impl.Repository.Profile.InternalUserDomainRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
            bool hasAccess = ad.IsInGroup("lmloggins", Svc.Core.Constants.ROLE_INTERNAL_DSR_FDF);

            Assert.IsTrue(hasAccess);
        }
    }
}
