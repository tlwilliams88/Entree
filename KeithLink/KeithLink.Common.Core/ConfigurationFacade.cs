using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
    public abstract class ConfigurationFacade
    {
        protected static string GetValue(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            return value != null ? value : defaultValue;
        }

        protected static string GetConnectionString(string key)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[key];
            if (connectionString == null)
                return null;
            return connectionString.ConnectionString;
        }
    }
}
