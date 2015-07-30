// KeithLink
using KeithLink.Svc.Core.Models.Profile;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ISettingsLogic {
        List<SettingsModel> GetAllUserSettings( Guid userId );
        void CreateOrUpdateSettings( SettingsModel settings );
        void DeleteSettings(SettingsModel settings);
    }
}
