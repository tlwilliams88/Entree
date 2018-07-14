using System;

namespace Entree.Core.Logging.Interfaces
{
    public interface IEventLogRepository
    {
        void WriteErrorLog(string logMessage);

        void WriteErrorLog(string logMessage, Exception ex);

        void WriteInformationLog(string logMessage);

        void WriteInformationLog(string logMessage, Exception ex);

        void WriteWarningLog(string logMessage);

        void WriteWarningLog(string logMessage, Exception ex);
    }
}
