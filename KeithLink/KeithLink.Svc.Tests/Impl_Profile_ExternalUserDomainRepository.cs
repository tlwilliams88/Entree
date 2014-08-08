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
            //bool success = ad.AuthenticateUser("sabroussard@benekeith.com", "L1ttleStev1e");
            bool success = ad.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void BelongsToGroup()
        {
            KeithLink.Svc.Impl.Profile.ExternalUserDomainRepository ad = new Impl.Profile.ExternalUserDomainRepository();
            bool hasAccess = ad.IsInGroup("sabroussard@somecompany.com", "Jimmys Chicken Shack Owner");

            Assert.IsTrue(hasAccess);
        }

        [TestMethod]
        public void CreateUser()
        {
            try
            {
                KeithLink.Svc.Impl.Profile.ExternalUserDomainRepository ad = new Impl.Profile.ExternalUserDomainRepository();

                ad.CreateUser("Jimmys Chicken Shack", "sabroussard@somecompany.com", "L1ttleStev1e", "Steven", "Broussard", Core.Constants.ROLE_EXTERNAL_OWNER);

                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }
    }
}
