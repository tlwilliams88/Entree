using KeithLink.Common.Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Enumerations;

namespace KeithLink.Svc.Test.Repositories.Mock
{
    public class AuditLogRepositoryMock : IAuditLogRepository
    {
        public AuditLogRepositoryMock()
        {
            Entries = new List<AuditLogEntry>();
        }
        public List<AuditLogEntry> Entries { get; set; }
        public void WriteToAuditLog(AuditType type, string actor = null, string information = null)
        {
            Entries.Add(new AuditLogEntry()
            {
                EntryType = type,
                Actor = actor,
                Information = information
            });
        }
    }
}
