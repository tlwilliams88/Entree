// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile {
    [DataContract(Name = "defaultorderlist")]
    [Serializable]
    public class DefaultOrderListModel
    {
        [DataMember( Name = "listid" )]
        public string ListId { get; set; }
    }
}
