using KeithLink.Svc.Core.Models.Messaging.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract]
    public class PushMessage
    {
        [DataMember(Name = "createdate")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "recipients")]
        public List<Recipient> Recipients { get; set; }

        [DataMember(Name = "message")]
        public Message Message { get; set; }

    }
}
