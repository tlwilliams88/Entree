using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Linq;
using KeithLink.Svc.Impl.Repository.Profile;
using IContainer = Autofac.IContainer;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class ProfileSettingsReporsitoryTest
    {
        #region Attributes

        private ISettingsRepository _repo;

        #endregion

        #region ctor

        public ProfileSettingsReporsitoryTest()
        {
            IContainer depMapo = DependencyMap.Build();
            _repo = depMapo.Resolve<ISettingsRepository>();

        }

        #endregion  

        #region tests

        [TestMethod]
        public void GetProfileSettings()
        {
            Guid userId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44");

            IQueryable<Settings> testSettings =  _repo.ReadByUser(userId);

            Assert.IsNotNull(testSettings.Count(), String.Format("Settings Not Found for user {0}.",userId));
        }

        [TestMethod]
        public void WriteProfileSetting()
        {

            Settings mySettings = new Settings  {
                                                    UserId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44"),
                                                    Key = "TestSetting",
                                                    Value = "Testing"
                                                };


            _repo.CreateOrUpdate(mySettings);

            IQueryable<Settings> newSettings = _repo.ReadByUser(mySettings.UserId);

            if (newSettings.Count() != 1)
            {
                Assert.Fail(String.Format("CreatedOrUpdate setting returned {0} for key {1} ", newSettings.Count(), mySettings.Key));
            }

        } 

        #endregion

    }
}
