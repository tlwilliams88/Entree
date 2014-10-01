using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic {
    [TestClass]
    public class UserProfileLogicImplTests {

        #region attrbitues
        private Common.Impl.Logging.EventLogRepositoryImpl                          _log;
        private Impl.Repository.Profile.ExternalUserDomainRepository                _extAd;
        private Impl.Repository.Profile.InternalUserDomainRepository                _intAd;
        private KeithLink.Svc.Impl.Repository.Profile.UserProfileRepository         _csProfileRepo;
        private Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository     _cache;
        #endregion

        #region ctor
        public UserProfileLogicImplTests() {
            _log = new Common.Impl.Logging.EventLogRepositoryImpl("KeithLink Unit Tests");

            _extAd = new Impl.Repository.Profile.ExternalUserDomainRepository(_log);
            _intAd = new Impl.Repository.Profile.InternalUserDomainRepository(_log);

            _cache = new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository();

            _csProfileRepo = new Impl.Repository.Profile.UserProfileRepository(_log, _extAd, _intAd, _cache);
        }
        #endregion

        [TestMethod]
        public void Ummmmm() {
            KeithLink.Svc.Impl.Logic.Profile.UserProfileLogicImpl logic = new Impl.Logic.Profile.UserProfileLogicImpl(_extAd, _intAd, _csProfileRepo);

            //Assert.IsTrue(logic.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e"));
        }

    }
}
