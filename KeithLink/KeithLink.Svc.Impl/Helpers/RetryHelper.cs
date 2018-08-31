using KeithLink.Common.Core.Interfaces.Logging;

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            Do<object>(() => { action(); return null; }, retryInterval, retryCount);
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 1; retry <= retryCount; retry++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }

        public static T Do<T>(Func<T> action, IEventLogRepository log, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 1; retry <= retryCount; retry++)
            {
                try
                {
                    return action();
                }
                catch (DbUpdateException exception)
                {
                    string entityNames = string.Empty;
                    foreach (var entry in exception.Entries)
                    {
                        if (entityNames != string.Empty)
                        {
                            entityNames += ", ";
                        }
                        var entity = entry.Entity;
                        var entityName = ObjectContext.GetObjectType(entity.GetType()).Name;
                        entityNames += entityName;
                    }
                    var errorMessage = string.Format("Could not persist changes to {0} with {1} attempts.", entityNames, retryCount);
                    log.WriteErrorLog(errorMessage, exception);

                    exceptions.Add(exception);
                    Thread.Sleep(retryInterval);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
