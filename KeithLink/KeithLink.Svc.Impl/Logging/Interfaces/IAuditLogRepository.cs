using KeithLink.Common.Core.Enumerations;

namespace Entree.Core.Logging.Interfaces
{
	public interface IAuditLogRepository
	{
		void WriteToAuditLog(AuditType type, string actor = null, string information= null);
	}
}
