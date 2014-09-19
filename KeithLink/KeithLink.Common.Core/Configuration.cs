using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
    public class Configuration : ConfigurationFacade
    {
        #region attributes
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
        private const string KEY_IS_PRODUCTION = "IsProduction";
        private const string KEY_LOGGING_CONNECTIONSTRING = "EventLog";
        private const string KEY_SMTP_FAILUREADDRESS = "FailureEmailAddress";
        private const string KEY_SMTP_FROMADDRESS = "FromEmailAddress";
        private const string KEY_SMTP_SERVERNAME = "SmtpServer";
        #endregion

        #region properties
        public static string FailureEmailAddress {
            get { return GetValue(KEY_SMTP_FAILUREADDRESS, string.Empty); }
        }

        public static string FromEmailAddress {
            get { return GetValue(KEY_SMTP_FROMADDRESS, string.Empty); }
        }

        public static bool IsProduction {
            get {
                bool retVal;

                bool.TryParse(GetValue(KEY_IS_PRODUCTION, "false"), out retVal);

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

        public static string SmtpServerAddress {
            get { return GetValue(KEY_SMTP_SERVERNAME, string.Empty); }
        }

        #endregion
    }
}
