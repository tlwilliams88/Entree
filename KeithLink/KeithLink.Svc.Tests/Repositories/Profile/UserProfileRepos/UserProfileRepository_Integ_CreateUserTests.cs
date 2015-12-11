// External
using Autofac;
// Keithlink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Cache;
// Core
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile.UserProfileRepos
{
    [TestClass]
    public class UserProfileRepository_Integ_CreateUserTests
    {
        #region attributes
        private IEventLogRepository _log;
        private IUserProfileRepository _profile;
        #endregion

        #region ctor
        public UserProfileRepository_Integ_CreateUserTests()
        {
            var container = DependencyMap.Build();
            _log = container.Resolve<IEventLogRepository>(new NamedParameter("applicationName", "Entree Test"));
            var _auditLog = container.Resolve<IAuditLogRepository>();
            _profile = container.Resolve<IUserProfileRepository>
                (new TypedParameter(typeof(IEventLogRepository), _log), new TypedParameter(typeof(IAuditLogRepository), _auditLog));
        }
        #endregion

        [TestMethod]
        public void Overall()
        {
            _profile.CreateUserProfile("devtest", "joesmith@company.com", "Joe", "Smith", "1234567890", "FEF");

            Assert.IsTrue(true);
        }
    }
}
