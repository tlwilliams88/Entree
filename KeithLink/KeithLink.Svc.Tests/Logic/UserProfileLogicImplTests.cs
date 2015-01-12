using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.WebApi.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;

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
        private AccountRepository                   _acct;
        private CustomerRepository _cust;
        #endregion

        #region ctor
        public UserProfileLogicImplTests() {
            _log = new Common.Impl.Logging.EventLogRepositoryImpl("KeithLink Unit Tests");
            _cache = new Impl.Repository.Profile.Cache.NoCacheUserProfileCacheRepository();
            var _custCach = new Impl.Repository.Profile.Cache.NoCacheCustomerCacheRepositoryImpl();
            
            _custRepo = new CustomerContainerRepository(_log);

            _extAd = new Impl.Repository.Profile.ExternalUserDomainRepository(_log, _custRepo);
            _intAd = new Impl.Repository.Profile.InternalUserDomainRepository(_log);


            _csProfileRepo = new Impl.Repository.Profile.UserProfileRepository(_log);

            _acct = new AccountRepository(_log, _custCach);
            _cust = new CustomerRepository(_log, _custCach);
            _logic = new UserProfileLogicImpl(_extAd, _intAd, _csProfileRepo, _cache, _acct, _cust, new NoOrderServiceRepositoryImpl(), new NoMessagingServiceRepositoryImpl(), new NoInvoiceServiceRepositoryImpl(), new EmailClientImpl(new TokenReplacer()), new NoMessagingServiceRepositoryImpl(), new EventLogRepositoryImpl("Test"));
        }
        #endregion

        [TestMethod]
        public void GetUserProfile() {
            Core.Models.Profile.UserProfileReturn userProfiles = _logic.GetUserProfile("jeremy@jeremyschickenshack.com");

            Assert.IsTrue(userProfiles.UserProfiles.Count == 1);
        }

    }
}
