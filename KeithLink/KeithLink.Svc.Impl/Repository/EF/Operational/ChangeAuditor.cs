using KeithLink.Common.Core.Interfaces.Logging;

using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
    public class ChangeAuditor
    {
        public static void AuditChanges(DbContext context, object entity, IEventLogRepository log)
        {
            DbEntityEntry entry = context.Entry(entity);
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var entityName = ObjectContext.GetObjectType(entity.GetType()).Name;
                var originalValue = entry.Property(propertyName).OriginalValue;
                var currentValue = entry.Property(propertyName).CurrentValue;
                var modified = entry.Property(propertyName).IsModified;

                var auditMessage = string.Format("The '{0}' of '{1}' was '{2}' is now '{3}' and modified is {4}.", propertyName, entityName, originalValue, currentValue, modified);
                log.WriteInformationLog(auditMessage);
            }
        }

    }
}
