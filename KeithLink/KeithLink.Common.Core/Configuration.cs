using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
    public class Configuration : ConfigurationFacade
    {
        private const string KEY_LOGGING_CONNECTIONSTRING = "LoggingConnection";
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
               
        public static string LoggingConnectionString
        {
            get 
            {
                //Returns the logging connection string if it exist.
                //If the logging connection string doesn't exist, it uses the AppData connection string

                var logConnectionString = GetConnectionString(KEY_LOGGING_CONNECTIONSTRING);
                if (logConnectionString == null)
                    return GetConnectionString(KEY_APPDATA_CONNECTIONSTRING);

                return logConnectionString; 
            }
        }

    }
}
