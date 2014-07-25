using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Logging
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
