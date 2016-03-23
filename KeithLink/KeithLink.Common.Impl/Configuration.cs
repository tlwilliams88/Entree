using KeithLink.Common.Impl.SettingsRepo;
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
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
        private const string KEY_IS_PRODUCTION = "IsProduction";
        private const string KEY_SMTP_FAILUREADDRESS = "FailureEmailAddress";
        private const string KEY_SMTP_FROMADDRESS = "FromEmailAddress";
        private const string KEY_SMTP_SERVERNAME = "SmtpServer";
        private const string KEY_APP_NAME = "AppName";
        private const string DEFAULT_APPNAME = "Entree";

        #endregion

        #region properties
		public static bool LogSystemPerformance
		{
			get
			{
				bool retVal;

				bool.TryParse(DBAppSettingsRepositoryImpl.GetValue(KEY_LOG_SYSTEM_PERFORMANCE, "false"), out retVal);

				return retVal;
			}
		}

		public static string AuditLogConnectionString
		{
			get { return GetConnectionString(KEY_AUDITLOG_CONNECTIONSTRING); }
		}
        public static string ApplicationName
        {
            get
            {
                return GetValue(KEY_APP_NAME, DEFAULT_APPNAME);
            }
        }
        public static string AppDataConnectionString
        {
            get { return GetConnectionString(KEY_APPDATA_CONNECTIONSTRING); }
        }

        public static string FailureEmailAddress
        {
            get { return DBAppSettingsRepositoryImpl.GetValue(KEY_SMTP_FAILUREADDRESS, string.Empty); }
        }

        public static string FromEmailAddress
        {
            get { return DBAppSettingsRepositoryImpl.GetValue(KEY_SMTP_FROMADDRESS, string.Empty); }
        }

        public static bool IsProduction
        {
            get
            {
                bool retVal;

                bool.TryParse(DBAppSettingsRepositoryImpl.GetValue(KEY_IS_PRODUCTION, "false"), out retVal);

                return retVal;
            }
        }

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

        public static string SmtpServerAddress
        {
            get { return DBAppSettingsRepositoryImpl.GetValue(KEY_SMTP_SERVERNAME, string.Empty); }
        }


        #endregion


    }
}
