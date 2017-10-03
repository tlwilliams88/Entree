using KeithLink.Common.Core.Interfaces.Logging;

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using BEKlibrary.EventLog.BusinessLayer;

namespace KeithLink.Common.Impl.Repository.Logging {
    public class EventLogQueueRepositoryImpl : IEventLogRepository {
        #region attributes
        private BEKlibrary.EventLog.Datalayer.QueueRepository _log;

        private const int SEVERITY_NOT_SET = -1;
        private const int SEVERITY_DEBUG = 0;
        private const int SEVERITY_INFORMATION = 1;
        private const int SEVERITY_WARNING = 2;
        private const int SEVERITY_ERROR = 3;
        #endregion

        #region ctor
        public EventLogQueueRepositoryImpl(string applicationName) {
            if (Configuration.LoggingConnectionString == null) {
                FailoverToWindowsEventLog("EventLog connection string was not found in the configuration file", null, EventLogEntryType.Warning);
            } else {
                _log = new BEKlibrary.EventLog.Datalayer.QueueRepository();
            }
        }
        #endregion

        #region methods
        private LogMessage GetLogMessage(string message, Exception ex, int severity) {
            LogMessage newMessage = new LogMessage();

            newMessage.Message = message;
            newMessage.EntryType.Id = severity;
            newMessage.Machine.Name = Environment.MachineName;

            newMessage.Application.Environment = ConfigurationHelper.GetActiveConfiguration();
            newMessage.Application.Name = ConfigurationManager.AppSettings["AppName"];

            if (Configuration.LogSystemPerformance) {
                newMessage.Machine.CpuUtilization = GetCurrentCPU();
                newMessage.Machine.MemoryUsed = GetCurrentRAMUsage();
            }

            if (System.Web.HttpContext.Current != null) {
                if (System.Web.HttpContext.Current.Request.LogonUserIdentity.Name != null) {
                    newMessage.UserName = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name;
                }
                newMessage.Web.HttpVerb = System.Web.HttpContext.Current.Request.HttpMethod;
                newMessage.Web.Url = System.Web.HttpContext.Current.Request.RawUrl;
                newMessage.Web.ClientIpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                newMessage.Web.Status = System.Web.HttpContext.Current.Response.StatusCode.ToString();
                newMessage.Method.Parameters.Add("UserSelectedContext", System.Web.HttpContext.Current.Request.Headers["userSelectedContext"]);
            }

            StringBuilder stackTrace = new StringBuilder();
            stackTrace.AppendLine(ex.Message);
            stackTrace.AppendLine();
            stackTrace.AppendLine("Exception Stack:");
            stackTrace.AppendLine("  Outer Stack:");
            stackTrace.AppendLine(ex.StackTrace);
            stackTrace.AppendLine();


            while (ex.InnerException != null) {
                stackTrace.AppendLine(ex.Message);
                stackTrace.AppendLine("  Inner Stack:");
                stackTrace.AppendLine(ex.StackTrace);
                stackTrace.AppendLine();
            }

            newMessage.Exception.Message = ex.Message;
            newMessage.Exception.StackTrace = stackTrace.ToString();

            return newMessage;
        }

        private LogMessage GetSingleMessageLog(string message, int severity) {
            LogMessage logMessage = new LogMessage();

            logMessage.Message = message;
            logMessage.EntryType.Id = severity;
            logMessage.Machine.Name = Environment.MachineName;
            logMessage.Application.Name = ConfigurationManager.AppSettings["AppName"];
            logMessage.Application.Environment = ConfigurationHelper.GetActiveConfiguration();

            return logMessage;
        }

        private string GetCurrentCPU() {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            //You need to read the NextValue twice, with 1 second in between each call: http://blogs.msdn.com/b/bclteam/archive/2006/06/02/618156.aspx
            //Because of this 1 secound delay, this method should only be called when troubleshooting a specific issue, turn off at other times
            Thread.Sleep(1000);
            return cpuCounter.NextValue()
                             .ToString();
        }

        private string GetCurrentRAMUsage() {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            ramCounter.NextValue();
            //You need to read the NextValue twice, with 1 second in between each call: http://blogs.msdn.com/b/bclteam/archive/2006/06/02/618156.aspx
            //Because of this 1 secound delay, this method should only be called when troubleshooting a specific issue, turn off at other times
            Thread.Sleep(1000);
            return ramCounter.NextValue()
                             .ToString();
        }

        public void WriteErrorLog(string logMessage) {
            try {
                _log.PublishLogMessage(logMessage);
            } catch {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Error);
            }
        }

        public void WriteErrorLog(string logMessage, Exception ex) {
            try {
                _log.PublishLogMessage(GetLogMessage(logMessage, ex, SEVERITY_ERROR));
            } catch {
                FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Error);
            }
        }

        public void WriteInformationLog(string logMessage) {
            try {
                _log.PublishLogMessage(GetSingleMessageLog(logMessage, SEVERITY_INFORMATION));
            } catch {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Information);
            }
        }

        public void WriteInformationLog(string logMessage, Exception ex) {
            try {
                _log.PublishLogMessage(GetLogMessage(logMessage, ex, SEVERITY_INFORMATION));
            } catch {
                FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Error);
            }
        }

        public void WriteWarningLog(string logMessage) {
            try {
                _log.PublishLogMessage(GetSingleMessageLog(logMessage, SEVERITY_WARNING));
            } catch {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Warning);
            }
        }

        public void WriteWarningLog(string logMessage, Exception ex) {
            try {
                _log.PublishLogMessage(GetLogMessage(logMessage, ex, SEVERITY_WARNING));
            } catch {
                FailoverToWindowsEventLog(logMessage, ex, EventLogEntryType.Warning);
            }
        }

        private void FailoverToWindowsEventLog(string logMessage, Exception ex, EventLogEntryType type) {
            string sSource = "BEK_KeithLink";
            string sLog = "Application";

            StringBuilder message = new StringBuilder(logMessage);

            if (ex != null) {
                message.AppendLine();
                message.Append(ex.ToString());
            }

            try {
                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                EventLog.WriteEntry(sSource, message.ToString(), type, 234);
            } catch { } //If this fails too, then just swallow the error. Logging should not be able to take down the entire site
        }
        #endregion
    }
}