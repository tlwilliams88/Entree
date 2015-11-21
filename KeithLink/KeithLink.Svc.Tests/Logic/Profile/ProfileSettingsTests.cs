// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Test.Mock; 

using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions;

// Core
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeithLink.Svc.Test.Logic.Profile {

    [TestClass]
    public class ProfileSettingsTests {

        #region attributes

        private ISettingsLogicImpl _settingsLogic;
        private IContainer _container;

        #endregion


        #region constructor

        [TestInitialize]
        public void init() {
            _container = DependencyMap.Build();

            _settingsLogic = _container.Resolve<ISettingsLogicImpl>();
        }

        public ProfileSettingsTests() {
        }

        #endregion

        #region test methods

        [TestMethod]
        public void SettingShouldGetCreated() {
            SettingsModel settings = new SettingsModel() {
                UserId = Guid.Parse( "d616546e-463a-45ba-b1d4-d3512a56ace7" ),
                Key = "TestSetting",
                Value = "TestValue"
            };

            _settingsLogic.CreateOrUpdateSettings( settings );
        }


        [TestMethod]
        public void SettingShouldGetUpdated() {
            SettingsModel settings = new SettingsModel() {
                UserId = Guid.Parse( "00000000-0000-0000-0000-000000000001"),
                Key = "Key1",
                Value = "UpdatedValue"
            };

            _settingsLogic.CreateOrUpdateSettings( settings );
        }

        [TestMethod]
        public void SettingShouldGetDeleted() {
            SettingsModel settings = new SettingsModel() {
                UserId = Guid.Parse( "00000000-0000-0000-0000-000000000001" ),
                Key = "Key1",
                Value = "Value1"
            };

            _settingsLogic.DeleteSettings( settings );
        }

        #endregion



    }
}
