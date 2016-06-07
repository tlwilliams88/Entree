using KeithLink.Common.Core.Enumerations;

namespace KeithLink.Common.Core.Interfaces.Logging
{
	public interface IAuditLogRepository
	{
		void WriteToAuditLog(AuditType type, string actor = null, string information= null);
	}
}
