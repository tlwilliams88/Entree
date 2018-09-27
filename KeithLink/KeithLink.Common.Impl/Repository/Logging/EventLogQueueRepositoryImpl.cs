using BEKlibrary;
using BEKlibrary.EventLog.BusinessLayer;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Models.Logging;

using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web;

namespace KeithLink.Common.Impl.Repository.Logging
{
    public class EventLogQueueRepositoryImpl : IEventLogRepository
    {
        #region attributes
        private IQueueRepository _queue;

        private const int SEVERITY_NOT_SET = -1;
        private const int SEVERITY_DEBUG = 0;
        private const int SEVERITY_INFORMATION = 1;
        private const int SEVERITY_WARNING = 2;
        private const int SEVERITY_ERROR = 3;
        #endregion

        #region ctor
        public EventLogQueueRepositoryImpl(IQueueRepository queueRepository)
        {
            _queue = queueRepository;
        }

        public EventLogQueueRepositoryImpl(string applicationName)
        {
            if (Configuration.LoggingConnectionString == null)
            {
                FailoverToWindowsEventLog("EventLog connection string was not found in the configuration file", null, EventLogEntryType.Warning);
            }
            else
            {
                _queue = new BEKlibrary.EventLog.Datalayer.QueueRepository();
            }
        }
        #endregion

        #region methods
        private LogMessage GetLogMessage(string message, int severity, TransactionContext context, Exception exception = null)
        {
            LogMessage newMessage = new LogMessage();

            newMessage.Message = message;
            newMessage.EntryType.Id = severity;
            newMessage.Machine.Name = Environment.MachineName;

            newMessage.Application.Environment = ConfigurationHelper.GetActiveConfiguration();
            newMessage.Application.Name = ConfigurationManager.AppSettings["AppName"];

            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                var httpRequest = httpContext.Request;
                if (httpRequest.LogonUserIdentity.Name != null)
                {
                    newMessage.UserName = httpRequest.LogonUserIdentity.Name;
                }
                newMessage.Web.HttpVerb = httpRequest.HttpMethod;
                newMessage.Web.Url = httpRequest.RawUrl;
                newMessage.Web.ClientIpAddress = httpRequest.UserHostAddress;
                newMessage.Method.Parameters.Add("UserSelectedContext", httpRequest.Headers["userSelectedContext"]);

                var httpResponse = httpContext.Response;
                newMessage.Web.Status = httpResponse.StatusCode.ToString();
            }

            if (context != null)
            {
                newMessage.Message = string.Format($"[{context.TransactionId}] {newMessage.Message}");
                newMessage.Method.Name = string.Format($"{context.ClassName}.{context.MethodName}");
            }

            if (exception != null)
            {
                newMessage.Exception.Message = exception.Message;
                newMessage.Exception.StackTrace = GetStackTrace(exception);

                if (Configuration.LogSystemPerformance)
                {
                    newMessage.Machine.CpuUtilization = GetCurrentCPU();
                    newMessage.Machine.MemoryUsed = GetCurrentRAMUsage();
                }
            }

            return newMessage;
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

            // This will iterate through all the inner exceptions and add them to the stack trace
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
            return cpuCounter.NextValue()
                             .ToString();
        }

        private string GetCurrentRAMUsage()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            ramCounter.NextValue();
            //You need to read the NextValue twice, with 1 second in between each call: http://blogs.msdn.com/b/bclteam/archive/2006/06/02/618156.aspx
            //Because of this 1 secound delay, this method should only be called when troubleshooting a specific issue, turn off at other times
            Thread.Sleep(1000);
            return ramCounter.NextValue()
                             .ToString();
        }

        public void WriteErrorLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_ERROR, context));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Error);
            }
        }

        public void WriteErrorLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_ERROR, context, exception));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Error);
            }
        }

        public void WriteInformationLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_INFORMATION, context));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Information);
            }
        }

        public void WriteInformationLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_INFORMATION, context, exception));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Error);
            }
        }

        public void WriteWarningLog(string logMessage, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_WARNING, context));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, null, EventLogEntryType.Warning);
            }
        }

        public void WriteWarningLog(string logMessage, Exception exception, TransactionContext context = null)
        {
            try
            {
                _queue.PublishLogMessage(GetLogMessage(logMessage, SEVERITY_WARNING, context, exception));
            }
            catch
            {
                FailoverToWindowsEventLog(logMessage, exception, EventLogEntryType.Warning);
            }
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