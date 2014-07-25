using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using KeithLink.Common.Core.Logging.Log4Net;

namespace KeithLink.Common.Core.Logging
{
	public class LogManager
	{
        public static void SetApplicationName(string name)
        {
            Log4NetLogger.ApplicationName = name;
        }

		public static ILogger GetLogger(Type loggerName)
		{
			return Log4NetLogger.GetLogger(loggerName);
		}
	}
}
