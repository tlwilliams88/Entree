using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging
{
    public class ProfileChannelModel
    {
        [DataMember(Name = "channel")]
        public Channel Channel { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}