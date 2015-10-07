using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile {
    [DataContract(Name = "usersettings")]
    public class SettingsModelReturn {
        [DataMember( Name = "key" )]
        public string Key { get; set; }

        [DataMember( Name = "value" )]
        public string Value { get; set; }
    }
}
