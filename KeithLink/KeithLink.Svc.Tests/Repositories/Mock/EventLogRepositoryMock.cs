using KeithLink.Common.Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Test.Repositories.Mock
{
    public class EventLogRepositoryMock : IEventLogRepository
    {
        public List<string> Errors { get; set; }
        public List<string> Infos { get; set; }
        public List<string> Warnings { get; set; }
        public EventLogRepositoryMock()
        {
            Errors = new List<string>();
            Infos = new List<string>();
            Warnings = new List<string>();
        }
        public void WriteErrorLog(string logMessage)
        {
            Errors.Add(logMessage);
        }

        public void WriteErrorLog(string logMessage, Exception ex)
        {
            Errors.Add(logMessage);
        }

        public void WriteInformationLog(string logMessage)
        {
            Infos.Add(logMessage);
        }

        public void WriteInformationLog(string logMessage, Exception ex)
        {
            Infos.Add(logMessage);
        }

        public void WriteWarningLog(string logMessage)
        {
            Warnings.Add(logMessage);
        }

        public void WriteWarningLog(string logMessage, Exception ex)
        {
            Warnings.Add(logMessage);
        }
    }
}
