using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Models.Logging;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web;

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
        private string GetLogMessage(string message, TransactionContext context, Exception exception = null)
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

            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                msg.AppendLine("This is a web request.");
                var httpRequest = httpContext.Request;
                msg.AppendLine(string.Format("Request is: \"{0}\"", httpRequest.RawUrl));
                msg.AppendLine(string.Format("Request type is: \"{0}\"", httpRequest.RequestType));
                msg.AppendLine(string.Format("User Selected Context is: \"{0}\"", httpRequest.Headers["userSelectedContext"]));
                msg.AppendLine();
            }

            if (exception != null)
            {
                if (exception.TargetSite != null)
                {
                    msg.AppendLine(string.Format("Target Error Routine is: \"{0}\"", exception.TargetSite.Name));
                    msg.AppendLine();
                }

                msg.AppendLine(GetStackTrace(exception));
            }

            return msg.ToString();
        }

        private string GetStackTrace(Exception exception)
        {
            StringBuilder stackTrace = new StringBuilder();

            stackTrace.AppendLine(exception.Message);
            stackTrace.AppendLine();
            stackTrace.AppendLine("Exception Stack:");
            stackTrace.AppendLine("  Outer Stack:");
            stackTrace.AppendLine(exception.StackTrace);
            stackTrace.AppendLine();

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                stackTrace.AppendLine(exception.Message);
                stackTrace.AppendLine("  Inner Stack:");
                stackTrace.AppendLine(exception.StackTrace);
                stackTrace.AppendLine();
            }

            return stackTrace.ToString();
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

        public void WriteErrorLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _log.WriteErrorLog(GetLogMessage(logMessage, context));
            }
            catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Error); }
        }

        public void WriteErrorLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _log.WriteErrorLog(GetLogMessage(logMessage, context, exception));
            }
            catch { FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Error); }
        }

        public void WriteInformationLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _log.WriteInformationLog(GetLogMessage(logMessage, context));
            }
            catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Information); }
        }

        public void WriteInformationLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _log.WriteInformationLog(GetLogMessage(logMessage, context, exception));
            }
            catch { FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Error); }
        }

        public void WriteWarningLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _log.WriteWarningLog(GetLogMessage(logMessage, context));
            }
            catch { FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Warning); }
        }

        public void WriteWarningLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _log.WriteWarningLog(GetLogMessage(logMessage, context, exception));
            }
            catch { FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Warning); }
        }

        private void FailoverToWindowsEventLog(string logMessage, Exception exception, EventLogEntryType type)
        {
            string sSource = "BEK_KeithLink";
            string sLog = "Application";

            StringBuilder message = new StringBuilder(logMessage);

            if (exception != null)
            {
                message.AppendLine();
                message.Append(exception.ToString());
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
