﻿// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class NoSettingsLogicImpl : ISettingsLogicImpl {

        public List<SettingsModelReturn> GetAllUserSettings( Guid userId ) {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateSettings( SettingsModel settings ) {
            throw new NotImplementedException();
        }

        public void DeleteSettings(SettingsModel settings)
        {
            throw new NotImplementedException();
        }

    }
}