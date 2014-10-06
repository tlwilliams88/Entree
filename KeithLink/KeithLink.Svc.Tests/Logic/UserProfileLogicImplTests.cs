﻿using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic {
    [TestClass]
    public class UserProfileLogicImplTests {

        #region attrbitues
        private NoCacheUserProfileCacheRepository   _cache;
        private UserProfileRepository               _csProfileRepo;
        private CustomerContainerRepository         _custRepo;
        private ExternalUserDomainRepository        _extAd;
        private InternalUserDomainRepository        _intAd;
        private EventLogRepositoryImpl              _log;
        private UserProfileLogicImpl                _logic;
        #endregion

        #region ctor
        public UserProfileLogicImplTests() {
            _log = new Common.Impl.Logging.EventLogRepositoryImpl("KeithLink Unit Tests");
            _cache = new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository();
            
            _custRepo = new CustomerContainerRepository(_log);

            _extAd = new Impl.Repository.Profile.ExternalUserDomainRepository(_log, _custRepo);
            _intAd = new Impl.Repository.Profile.InternalUserDomainRepository(_log);


            _csProfileRepo = new Impl.Repository.Profile.UserProfileRepository(_log, _cache);

            _logic = new UserProfileLogicImpl(_extAd, _intAd, _csProfileRepo, _cache);
        }
        #endregion

        [TestMethod]
        public void Ummmmm() {
            //Assert.IsTrue(_logic.AuthenticateUser("sabroussard@somecompany.com", "L1ttleStev1e"));
        }

    }
}
