// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models {
    [DataContract(Name = "user_settings")]
    [Serializable]
    public class SettingsModel {
        [DataMember( Name = "userId" )]
        public Guid UserId { get; set; }

        [DataMember( Name = "key" )]
        public string Key { get; set; }

        [DataMember( Name = "value" )]
        public string Value { get; set; }
    }
}
