using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    public abstract class BaseNotification
    {
        public string CustomerNumber { get; set; }
        public abstract MessageType MessageType { get; }
    }
}
