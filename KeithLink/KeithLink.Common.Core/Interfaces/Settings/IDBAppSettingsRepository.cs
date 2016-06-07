using KeithLink.Common.Core.Models.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Interfaces.Settings {
    public interface IDBAppSettingsRepository {
        Setting Read(string key);

        List<Setting> ReadAll();

        void Update(string key, string value, string comment,
                    bool disabled);
    }
}
