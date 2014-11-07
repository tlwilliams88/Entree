using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KeithLink.Svc.Core.Models.Messaging
{
    [DataContract(Name = "messagepagingmodel")]
    public class MessagePagingModel
    {

        [DataMember(Name = "from")]
        public int From { get; set; }
        [DataMember(Name = "size")]
        public int Size { get; set; }

    }
}