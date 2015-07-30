using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;

using Autofac;
using IContainer = Autofac.IContainer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using Autofac.Core;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class ProfileSettingsReporsitoryTest
    {
        #region Attributes

        private ISettingsRepository _repo;
        private IUnitOfWork _uow;

        #endregion

        #region ctor

        public ProfileSettingsReporsitoryTest()
        {
            IContainer depMapo = DependencyMap.Build();
            _repo = depMapo.Resolve<ISettingsRepository>();
            _uow = depMapo.Resolve<IUnitOfWork>();
        }

        #endregion  

        #region tests

        [TestMethod]
        public void ReadProfileSettings()
        {
            Guid userId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44");

            IQueryable<Settings> mySettings = _repo.ReadByUser(userId);

            Assert.IsTrue(mySettings != null);
        }

        [TestMethod]
        public void CreateProfileSetting()
        {

            SettingsModel settings = new SettingsModel  { UserId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44"),
                                                             Key = "TestSetting",
                                                           Value = "Testing" };
            Settings mySettings = settings.ToEFSettings();

            _repo.CreateOrUpdate(mySettings);
            _uow.SaveChanges();

            //Verify Setting
            IQueryable<Settings> newSettings = _repo.ReadByUser(mySettings.UserId).Where(s => s.UserId == mySettings.UserId &&
                                                                                              s.Key    == mySettings.Key );
            if (!newSettings.Any())
            {
                Assert.Fail("CreatProfileSetting failed: Could not retrieve created setting");
            }

            //Delete Setting
            _repo.Delete(s => s.UserId == settings.UserId &&
                               s.Key == settings.Key &&
                               s.Value == settings.Value);
            _uow.SaveChanges();
            // Test to be sure it's deleted.
            newSettings = _repo.ReadByUser(mySettings.UserId).Where(s => s.UserId == settings.UserId &&
                                                                         s.Key == settings.Key);
            if (newSettings.Any())
            {
                Assert.Fail("UpdateProfileSetting failed: Deleted setting still exists");
            }
        }

        [TestMethod]
        public void UpdateProfileSettings()
        {
            // Create Profile Setting()
            SettingsModel settings = new SettingsModel
            {
                UserId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44"),
                Key = "TestSetting",
                Value = "Testing"
            };
            Settings mySettings = settings.ToEFSettings();

            _repo.CreateOrUpdate(mySettings);
            _uow.SaveChanges();

            // Validate Setting Created
            IQueryable<Settings> newSettings = _repo.ReadByUser(mySettings.UserId).Where(k => mySettings.Key == settings.Key);
            if (!newSettings.Any())
            {
                Assert.Fail("UpdateProfileSetting failed: Could not retrieve created setting");
            }

            // Update Settings
            settings.Value = "Updated Settings";
            mySettings = settings.ToEFSettings();
            _repo.CreateOrUpdate(mySettings);

            // Verify Settings Updated
            newSettings = _repo.ReadByUser(mySettings.UserId).Where(k => mySettings.Key == settings.Key);
            if (!newSettings.Any())
            {
                Assert.Fail("UpdateProfileSetting failed: Could not retrieve created setting");
            }

            //Delete Setting
            _repo.Delete(s => s.UserId == settings.UserId &&
                               s.Key == settings.Key &&
                               s.Value == settings.Value);
            _uow.SaveChanges();

            // Test to be sure it's deleted.
            newSettings = _repo.ReadByUser(mySettings.UserId).Where(k => mySettings.Key == settings.Key && mySettings.Key == settings.Key);
            if (!newSettings.Any())
            {
                Assert.Fail("UpdateProfileSetting failed: Deleted setting still exists");
            }

        }

        [TestMethod]
        public void DeleteProfileSetting()
        {
            SettingsModel settings = new SettingsModel
            {
                UserId = new Guid("b1c2887b-c406-48a0-8c61-9d944b3a7e44"),
                Key = "TestSetting",
                Value = "Testing"
            };
            Settings mySettings = settings.ToEFSettings();
            
            //Create Setting
            _repo.CreateOrUpdate(mySettings);
            _uow.SaveChanges();

            //Verify Setting
            IQueryable<Settings> newSettings = _repo.ReadByUser(mySettings.UserId).Where(k => mySettings.Key == settings.Key);
            if (!newSettings.Any())
            {
                Assert.Fail("DeleteProfileSetting failed: Could not retrieve created setting");
            }

            //Delete Setting
            _repo.Delete(s => s.UserId == settings.UserId &&
                               s.Key == settings.Key &&
                               s.Value == settings.Value);
            _uow.SaveChanges();

            // Test to be sure it's deleted.
            newSettings = _repo.ReadByUser(mySettings.UserId).Where(k => mySettings.Key == settings.Key && mySettings.Key == settings.Key);
            if (!newSettings.Any())
            {
                Assert.Fail("DeleteProfileSetting failed: Could not retrieve created setting");
            }
        }

        #endregion

    }
}
