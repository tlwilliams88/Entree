using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.EF;

namespace KeithLink.Svc.Core.Models.Messaging
{
    public class UserMessage : BaseEFModel
    {
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public string MessageType { get; set; }
        public DateTime? MessageReadUtc { get; set; }
    }
}
