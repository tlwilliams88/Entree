using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="etanotification")]
    public class EtaNotification : BaseNotification
    {
        public EtaNotification()
        {
            this.NotificationType = NotificationType.Eta;
        }

        [DataMember(Name = "orders")]
        public List<OrderEta> Orders { get; set; }
    }
}
