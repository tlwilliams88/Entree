using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.OnlinePayments;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.WebApi.Repository.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic {
    [TestClass]
    public class UserProfileLogicImplTests {

        #region attrbitues
		private NoCacheRepositoryImpl _cache;
        private UserProfileRepository               _csProfileRepo;
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
			_cache = new NoCacheRepositoryImpl();
			var _custCach = new NoCacheRepositoryImpl();
            var dsrService = new NoDsrServiceRepository();
            
            _extAd = new Impl.Repository.Profile.ExternalUserDomainRepository(_log);
            _intAd = new Impl.Repository.Profile.InternalUserDomainRepository(_log);


            _csProfileRepo = new Impl.Repository.Profile.UserProfileRepository(_log);

            _acct = new AccountRepository(_log);
            _cust = new CustomerRepository(_log, _custCach, dsrService);
            _logic = new UserProfileLogicImpl(_extAd, _intAd, _csProfileRepo, _cache, _acct, _cust, new NoOrderServiceRepositoryImpl(), new NoMessagingServiceRepositoryImpl(), new NoInvoiceServiceRepositoryImpl(), new EmailClientImpl(), new NoMessagingServiceRepositoryImpl(), new EventLogRepositoryImpl("Test"), new NoOnlinePaymentServiceRepository(), new GenericQueueRepositoryImpl());
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetUserProfile() {
            Core.Models.Profile.UserProfileReturn userProfiles = _logic.GetUserProfile("sabroussard@somecompany.com");

            Assert.IsTrue(userProfiles.UserProfiles.Count == 1);
        }

        [TestMethod]
        public void SuccessfullyCreateGuest() {
            //_logic.CreateGuestUserAndProfile("testguest@somecompany.com", "Ab12345", "FDF");
        }

        [TestMethod]
        public void SuccessfullyCreateGuestWithTempPassword() {
            //_logic.UserCreatedGuestWithTemporaryPassword("usercreatedguest@somecompany.com", "FDF");
        }

        public void SuccssfulCreateUserAndProfile() {
            //_logic.CreateUserAndProfile("Jeremys Chicken Shack", "test@somecompany.com", "Ab12345", "Test", "User", "", "accounting", "FDF");
        }

        [TestMethod]
        public void SuccessfullyGrantAccessToKbitCustomer() {
            _logic.GrantRoleAccess("sabroussard@somecompany.com", Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer);
        }

        [TestMethod]
        public void SuccessfullyRemoveAccessToKbitCustomer() {
            _logic.RevokeRoleAccess("sabroussard@somecompany.com", Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer);
        }
        #endregion
    }
}
