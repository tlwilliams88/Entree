using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ApplicationHealth
{
    public class QueueToCheckModel
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string LogicalName { get; set; }
        public string Queue { get; set; }
        public int MaximumMessagesWarningThreshold { get; set; }
        public int MinimumConsumersWarningThreshold { get; set; }
    }
}
