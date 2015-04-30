using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl
{
    public class Configuration : KeithLink.Common.Core.ConfigurationFacade
    {
        #region attributes
        private const string KEY_LOGGING_CONNECTIONSTRING = "EventLog";
		private const string KEY_LOG_SYSTEM_PERFORMANCE = "LogSystemPerformanceWithErrors";
		private const string KEY_AUDITLOG_CONNECTIONSTRING = "AuditLog";
        
        #endregion

        #region properties
        public static string LoggingConnectionString
        {
            get
            {
                ////Returns the logging connection string if it exist.
                ////If the logging connection string doesn't exist, it uses the AppData connection string

                //var logConnectionString = GetConnectionString(KEY_LOGGING_CONNECTIONSTRING);
                //if (logConnectionString == null)
                //    return GetConnectionString(KEY_APPDATA_CONNECTIONSTRING);

                //return logConnectionString;

                return GetConnectionString(KEY_LOGGING_CONNECTIONSTRING);
            }
        }

		public static bool LogSystemPerformance
		{
			get
			{
				bool retVal;

				bool.TryParse(GetValue(KEY_LOG_SYSTEM_PERFORMANCE, "false"), out retVal);

				return retVal;
			}
		}

		public static string AuditLogConnectionString
		{
			get { return GetConnectionString(KEY_AUDITLOG_CONNECTIONSTRING); }
		}

        #endregion


    }
}
