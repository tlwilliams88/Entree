using KeithLink.Utility.NotificationParser.Models;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.NotificationParser.Repository {
    public class NotificationContext : DbContext {
        public DbSet<EtaNotification> Notifications { get; set; }
    }
}
