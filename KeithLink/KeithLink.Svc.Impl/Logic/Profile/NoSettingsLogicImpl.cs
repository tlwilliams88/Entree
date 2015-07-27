// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class NoSettingsLogicImpl : ISettingsLogic {

        public List<SettingsModel> GetAllUserSettings( Guid userId ) {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateSettings( SettingsModel settings ) {
            throw new NotImplementedException();
        }

    }
}
