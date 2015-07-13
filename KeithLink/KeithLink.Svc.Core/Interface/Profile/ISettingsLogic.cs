// KeithLink
using KeithLink.Svc.Core.Models.Profile.EF;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ISettingsLogic {
        List<Settings> GetAllUserSettings( string userId );
        Settings GetUserSetting( string userId, string key );
        void CreateOrUpdateSettings( Settings settings );
    }
}
