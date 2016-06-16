using KeithLink.Common.Core.Models.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Interfaces.Settings {
    public interface IAppSettingLogic {
        List<Setting> ReadAllSettings();
        SettingUpdate SaveSetting(string key, string value);
        List<SettingUpdate> SaveSettings(List<Setting> settings);
    }
}
