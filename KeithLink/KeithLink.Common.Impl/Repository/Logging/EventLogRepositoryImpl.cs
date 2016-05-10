﻿using KeithLink.Common.Core.Interfaces.Logging;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace KeithLink.Common.Impl.Repository.Logging
{
    public class EventLogRepositoryImpl : IEventLogRepository
    {
        #region attributes
        private BEKlibrary.EventLog.BusinessLayer.LogEntry _log;
        #endregion

        #region ctor
        public EventLogRepositoryImpl(string applicationName)
        {
            if (Configuration.LoggingConnectionString == null)
                FailoverToWindowsEventLog("EventLog connection string was not found in the configuration file", null, EventLogEntryType.Warning);
            else
                _log = new BEKlibrary.EventLog.BusinessLayer.LogEntry(Environment.MachineName, applicationName);
        }
        #endregion

        #region methods
        private string GetLogMessage(string message, Exception ex)
        {
            StringBuilder msg = new StringBuilder();
					
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

				msg.AppendLine(ex.Message);
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
			try
			{
				_log.WriteErrorLog(logMessage);
			}
			catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Error); }
        }

        public void WriteErrorLog(string logMessage, Exception ex)
        {
			try{
				_log.WriteErrorLog(GetLogMessage(logMessage, ex));
			}
			catch { FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Error); }
        }

        public void WriteInformationLog(string logMessage)
        {
			try{
				_log.WriteInformationLog(logMessage);
			}
			catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Information); }
        }

        public void WriteInformationLog(string logMessage, Exception ex)
        {
			try
			{
				_log.WriteInformationLog(GetLogMessage(logMessage, ex));
			}
			catch { FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Error); }
        }

        public void WriteWarningLog(string logMessage)
        {
			try{
				_log.WriteWarningLog(logMessage);
			}
			catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Warning); }
        }

        public void WriteWarningLog(string logMessage, Exception ex)
        {
			try{
				_log.WriteWarningLog(GetLogMessage(logMessage, ex));
			}
			catch { FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Warning); }
        }

		private void FailoverToWindowsEventLog(string logMessage, Exception ex, EventLogEntryType type)
		{
			string sSource = "BEK_KeithLink";
			string sLog = "Application";

			StringBuilder message = new StringBuilder(logMessage);

			if (ex != null)
			{
				message.AppendLine();
				message.Append(ex.ToString());
			}

			try
			{
				if (!EventLog.SourceExists(sSource))
					EventLog.CreateEventSource(sSource, sLog);

				EventLog.WriteEntry(sSource, message.ToString(), type, 234);
			}
			catch { } //If this fails too, then just swallow the error. Logging should not be able to take down the entire site
		}

        #endregion
    }
}
