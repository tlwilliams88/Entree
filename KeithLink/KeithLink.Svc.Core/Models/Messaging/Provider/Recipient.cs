using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.Provider
{
    public class Recipient
    {
        public string ProviderEndpoint { get; set; }
        public Guid UserId { get; set; }
        public string CustomerNumber { get; set; }
        public DeviceOS? DeviceOS { get; set; }
        public Channel Channel { get; set; }
    }
}
