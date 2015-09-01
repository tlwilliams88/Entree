// KeithLink
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Core.Models.Profile;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions {
    public static class SettingsExtensions {

        public static Settings ToEFSettings( this SettingsModel model ) {
            return new Settings() {
                UserId = model.UserId,
                Key = model.Key,
                Value = model.Value,
            };
        }

        public static SettingsModel ToModel( this Settings model ) {
            return new SettingsModel() {
                UserId = model.UserId,
                Key = model.Key,
                Value = model.Value,
            };
        }

        public static SettingsModelReturn ToReturnModel( this SettingsModel model ) {
            return new SettingsModelReturn() {
                Key = model.Key,
                Value = model.Value,
            };
        }

        public static SettingsModelReturn ToReturnModel( this Settings model ) {
            return new SettingsModelReturn() {
                Key = model.Key,
                Value = model.Value,
            };
        }


    }
}
