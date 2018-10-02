using KeithLink.Common.Core.Models.Logging;

using System;

namespace KeithLink.Common.Core.Interfaces.Logging
{
    public interface IEventLogRepository
    {
        void WriteErrorLog(string logMessage, TransactionContext context = null);

        void WriteErrorLog(string logMessage, Exception ex, TransactionContext context = null);

        void WriteInformationLog(string logMessage, TransactionContext context = null);

        void WriteInformationLog(string logMessage, Exception ex, TransactionContext context = null);

        void WriteWarningLog(string logMessage, TransactionContext context = null);

        void WriteWarningLog(string logMessage, Exception ex, TransactionContext context = null);
    }
}
