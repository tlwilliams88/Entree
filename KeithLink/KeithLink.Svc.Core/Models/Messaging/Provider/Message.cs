using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging.Provider
{
    public class Message
    {
        public string MessageBody { get; set; }
        public string MessageSubject { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public Svc.Core.Enumerations.Messaging.NotificationType NotificationType { get; set; }
    }
}
