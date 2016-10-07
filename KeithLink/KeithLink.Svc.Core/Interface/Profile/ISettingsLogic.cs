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
        List<SettingsModelReturn> GetAllUserSettings( Guid userId );

        void CreateOrUpdateSettings( SettingsModel settings );

        SettingsModelReturn GetUserCustomerDefaultOrderList(Guid userId, string customernumber, string branchid);

        void CreateOrUpdateUserCustomerDefaultOrderList(string customernumber, string branchid, SettingsModel settings);

        void DeleteSettings(SettingsModel settings);

        void SetDefaultApplicationSettings(string email);
    }
}
