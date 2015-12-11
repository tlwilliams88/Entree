// External
using Autofac;
using IContainer = Autofac.IContainer;
// KeithLink
using KeithLink.Common.Impl.AuditLog;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.Profile;
// Core
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.AuditLog;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Test.Repositories.Profile.ExternalUserDomainRepoTests
{
    [TestClass]
    // test scope - integration
    public class ExternalUserDomainRepository_CreateUserTests
    {
        #region attributes
        ICustomerDomainRepository _custUserRepo;
        IEventLogRepository _log;
        #endregion

        #region ctor
        public ExternalUserDomainRepository_CreateUserTests()
        {
            var container = DependencyMap.Build();
            //_logic = container.Resolve<ICustomerDomainRepository>(); 
            // TODO IoC resolve
            _log = container.Resolve<IEventLogRepository>
                (new TypedParameter(typeof(string), "Unit Tests"));
            IAuditLogRepository _auditLog = container.Resolve<IAuditLogRepository>();
            //_custUserRepo = new ExternalUserDomainRepository(_log, _auditLog);
            _custUserRepo = container.Resolve<ICustomerDomainRepository>
                (new TypedParameter(typeof(IEventLogRepository), _log),
                 new TypedParameter(typeof(IAuditLogRepository), _auditLog));
        }
        #endregion

        #region test
        [TestMethod]
        public void TryToCreateBenekeithUserDenied()
        {
            const string USER_EMAIL = "deletableuser@benekeith.com";

            string acno = null;
            try
            {
                acno = _custUserRepo.CreateUser("Test User", USER_EMAIL, "Ab12345", "First", "Last", Configuration.RoleNameOwner);
            }
            catch { }
            
            Assert.IsNull(acno); //customer account num is null so createuser failed
            if ((acno != null) && (acno.Length > 0)) 
            { // if user created cleanup user that was created
                _custUserRepo.DeleteUser(USER_EMAIL);
            }
        }
        [TestMethod]
        public void TryToCreateBenekeithUserThrowsError()
        {
            const string USER_EMAIL = "deletableuser@benekeith.com";

            string acno = null;
            try
            {
                acno = _custUserRepo.CreateUser("Test User", USER_EMAIL, "Ab12345", "First", "Last", Configuration.RoleNameOwner);
            }
            catch (KeithLink.Svc.Core.Exceptions.Profile.TriedToCreateInternalUserAsExternalException cex)
            {
                Assert.IsTrue(true); // created proper type of exception
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false); // created some other exception
            }
            if ((acno != null) && (acno.Length > 0))
            { // if user created cleanup user that was created
                _custUserRepo.DeleteUser(USER_EMAIL);
            }
        }
        [TestMethod]
        public void CreateAndDeleteUser() {
            const string USER_EMAIL = "deletableuser@somecompany.com";

            _custUserRepo.CreateUser("Jimmys Chicken Shack", USER_EMAIL, "Ab12345", "First", "Last", Configuration.RoleNameOwner);

            Assert.IsNotNull(_custUserRepo.GetUser(USER_EMAIL));

            _custUserRepo.DeleteUser(USER_EMAIL);

            Assert.IsNull(_custUserRepo.GetUser(USER_EMAIL));
        }
        #endregion
    }
}
