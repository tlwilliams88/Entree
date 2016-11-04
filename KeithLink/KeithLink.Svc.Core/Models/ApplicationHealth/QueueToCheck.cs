using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.ApplicationHealth
{
    public class QueueToCheck
    {
        [DataMember(Name = "server")]
        public string Server { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "logicalname")]
        public string LogicalName { get; set; }

        [DataMember(Name = "virtualhost")]
        public string VirtualHost { get; set; }

        [DataMember(Name = "queue")]
        public string Queue { get; set; }
    }
}
