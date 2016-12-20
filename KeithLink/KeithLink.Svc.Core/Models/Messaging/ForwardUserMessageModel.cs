using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging
{
    [DataContract(Name = "forwardusermessagemodel")]
    public class ForwardUserMessageModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "emailaddress")]
        public string EmailAddress { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
