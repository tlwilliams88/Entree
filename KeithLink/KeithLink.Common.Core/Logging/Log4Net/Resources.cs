using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Logging.Log4Net
{
    public static class Resources
    {
        public static string TOKEN_DATABASE_NAME = "${databaseName}";
        static string TOKEN_CONNECTION_STRING = "${connectionstring}";

        public static string CreateLogDatabaseSql { get { return LoggingResources.CreateLogDatabase_SQL; } }
        public static string CreateLogTableSql { get { return LoggingResources.CreateLogTable_SQL; } }
    }
}
