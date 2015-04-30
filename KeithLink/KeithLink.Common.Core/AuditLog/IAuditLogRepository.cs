using KeithLink.Common.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.AuditLog
{
	public interface IAuditLogRepository
	{
		void WriteToAuditLog(AuditType type, string actor = null, string information= null);
	}
}
