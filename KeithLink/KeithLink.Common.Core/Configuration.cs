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
        private const string KEY_LOGGING_CONNECTIONSTRING = "EventLog";
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
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
        #endregion


    }
}
