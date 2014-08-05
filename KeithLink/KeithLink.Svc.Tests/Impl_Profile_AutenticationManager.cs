using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_AutenticationManager
    {
        [TestMethod]
        public void CanAuthenticate()
        {
            bool success = KeithLink.Svc.Impl.Profile.AuthenticationManager.AuthenticateUser("sabroussard", "L1ttleStev1e");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup()
        {
            bool hasAccess = KeithLink.Svc.Impl.Profile.AuthenticationManager.IsInGroup("sabroussard", "Owner");

            Assert.IsTrue(hasAccess);
        }
    }
}
