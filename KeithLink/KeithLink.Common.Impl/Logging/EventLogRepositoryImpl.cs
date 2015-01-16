using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.Logging
{
    public class EventLogRepositoryImpl : KeithLink.Common.Core.Logging.IEventLogRepository
    {
        #region attributes
        private BEKlibrary.EventLog.BusinessLayer.LogEntry _log;
        #endregion

        #region ctor
        public EventLogRepositoryImpl(string applicationName)
        {
            if (Configuration.LoggingConnectionString == null)
                throw new ArgumentNullException("EventLog connection string was not found in the configuration file");
            else
                _log = new BEKlibrary.EventLog.BusinessLayer.LogEntry(Environment.MachineName, applicationName);
        }
            
        #endregion

        #region methods
        private string GetLogMessage(string message, Exception ex)
        {
            System.Text.StringBuilder msg = new StringBuilder();
					
			msg.Append(message);
            msg.AppendLine(":");

			if (Configuration.LogSystemPerformance)
			{
				msg.AppendFormat("Current System CPU %: {0}%", GetCurrentCPU());
				msg.AppendLine();
				msg.AppendFormat("Current System RAM Usage: {0}MB", GetCurrentRAMUsage());
				msg.AppendLine();
			}

			msg.AppendLine(ex.Message);
            msg.AppendLine();
            msg.AppendLine("Exception Stack:");
            msg.AppendLine("  Outer Stack:");
            msg.AppendLine(ex.StackTrace);
            msg.AppendLine();


            while (ex.InnerException != null)
            {
                ex = ex.InnerException;

                msg.AppendLine("  Inner Stack:");
                msg.AppendLine(ex.StackTrace);
                msg.AppendLine();
            }

            return msg.ToString();
        }

		private string GetCurrentCPU()
		{
			var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			cpuCounter.NextValue();
			//You need to read the NextValue twice, with 1 second in between each call: http://blogs.msdn.com/b/bclteam/archive/2006/06/02/618156.aspx
			//Because of this 1 secound delay, this method should only be called when troubleshooting a specific issue, turn off at other times
			Thread.Sleep(1000);
			return cpuCounter.NextValue().ToString();
		}

		private string GetCurrentRAMUsage()
		{
			var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
			ramCounter.NextValue();
			//You need to read the NextValue twice, with 1 second in between each call: http://blogs.msdn.com/b/bclteam/archive/2006/06/02/618156.aspx
			//Because of this 1 secound delay, this method should only be called when troubleshooting a specific issue, turn off at other times
			Thread.Sleep(1000);
			return ramCounter.NextValue().ToString();
		}

        public void WriteErrorLog(string logMessage)
        {
            _log.WriteErrorLog(logMessage);
        }

        public void WriteErrorLog(string logMessage, Exception ex)
        {
            _log.WriteErrorLog(GetLogMessage(logMessage, ex));
        }

        public void WriteInformationLog(string logMessage)
        {
            _log.WriteInformationLog(logMessage);
        }

        public void WriteInformationLog(string logMessage, Exception ex)
        {
            _log.WriteInformationLog(GetLogMessage(logMessage, ex));
        }

        public void WriteWarningLog(string logMessage)
        {
            _log.WriteWarningLog(logMessage);
        }

        public void WriteWarningLog(string logMessage, Exception ex)
        {
            _log.WriteWarningLog(GetLogMessage(logMessage, ex));
        }
        #endregion
    }
}
