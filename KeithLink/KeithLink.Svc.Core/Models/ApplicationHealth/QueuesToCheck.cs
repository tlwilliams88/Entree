using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.ApplicationHealth
{
    public class QueuesToCheck
    {
        [DataMember(Name = "targets")]
        public List<QueueToCheck> Targets { get; set; }
    }
}
