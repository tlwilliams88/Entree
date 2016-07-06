using KeithLink.Common.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Test.Repositories.Mock
{
    public class AuditLogEntry
    {
        public AuditType EntryType { get; set; }
        public string Actor { get; set; }
        public string Information { get; set; }
    }
}
