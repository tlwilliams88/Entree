using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using System.Configuration;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using System.Web;
using System.Data.SqlClient;

namespace KeithLink.Common.Core.Logging.Log4Net
{
	public class Log4NetLogger : ILogger
	{
        public static string ApplicationName { get; set; }

		ILog log = null;

        private const string FILE_CONFIGURATION_DOCUMENT = "log4netConfig.xml";

        static object locker = new object();
        private static bool isConfigured = false;

        //private static string ConfigurationFilePath
        //{
        //    get
        //    {
        //        try
        //        {
        //            string appRoot = Environment.GetEnvironmentVariable("RoleRoot");
        //            return Path.Combine(appRoot + @"\", string.Format(@"approot{0}\{1}"
        //                , System.Web.HttpContext.Current == null ? string.Empty : @"\bin"
        //                , FILE_CONFIGURATION_DOCUMENT));
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}

		private Log4NetLogger(ILog log)
		{
			this.log = log;
		}

		public static Log4NetLogger GetLogger(Type loggerName)
		{
			lock (locker)
			{
				if (!isConfigured)
				{
                    // configure logging
                    ConfigureLogging();
					isConfigured = true;
				}
            }

            ThreadContext.Properties["user"] = HttpContext.Current != null 
                && HttpContext.Current.User != null
                && HttpContext.Current.User.Identity.IsAuthenticated
                    ? HttpContext.Current.User.Identity.Name
                    : string.Empty;

			return new Log4NetLogger(log4net.LogManager.GetLogger(loggerName));
		}

        private static void ConfigureLogging()
        {
            //string configPath = ConfigurationFilePath;
            
            //if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            //{
                // get configuration document           
                string configurationDoc = LoggingResources.log4netConfig_XML;

                // inject connection string
                //ConnectionStringSettings dbConnectionConfig = ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_NAME_OPERATIONAL_CONTEXT];
                //if (dbConnectionConfig != null)
                //{
                //    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(dbConnectionConfig.ConnectionString);
                //    builder.InitialCatalog = string.Format("{0}.log", builder.InitialCatalog);
                //    builder.Remove("AttachDbFilename");
                //    configurationDoc = configurationDoc.Replace("${connectionstring}", builder.ConnectionString);
                //}

                // configure logging
                log4net.Config.XmlConfigurator.Configure(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(configurationDoc)));
               
                // add global context logging variables 
                GlobalContext.Properties["application"] = ApplicationName; 
                GlobalContext.Properties["host"] = Environment.MachineName; 

                // log initialization data
                Log4NetLogger log = new Log4NetLogger(log4net.LogManager.GetLogger(typeof(Log4NetLogger)));
                //log.DebugFormat("Log database connection string '{0}'", dbConnectionConfig == null ? "<NULL>" : dbConnectionConfig.ConnectionString);
            //}
        }

		#region ILogger Members

		public void Debug(object message)
		{
			log.Debug(message);
		}

		public void Debug(object message, Exception exception)
		{
			log.Debug(message, exception);
		}

		public void DebugFormat(string format, object arg0)
		{
			log.DebugFormat(format, arg0);
		}

		public void DebugFormat(string format, params object[] args)
		{
			log.DebugFormat(format, args);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			log.DebugFormat(provider, format, args);
		}

		public void DebugFormat(string format, object arg0, object arg1)
		{
			log.DebugFormat(format, arg0, arg1);
		}

		public void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			log.DebugFormat(format, arg0, arg1, arg2);
		}

		public void Error(object message)
		{
			log.Error(message);
		}

		public void Error(object message, Exception exception)
		{
			log.Error(message, exception);
		}

		public void ErrorFormat(string format, object arg0)
		{
			log.ErrorFormat(format, arg0);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			log.ErrorFormat(format, args);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			log.ErrorFormat(provider, format, args);
		}

		public void ErrorFormat(string format, object arg0, object arg1)
		{
			log.ErrorFormat(format, arg0, arg1);
		}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			log.ErrorFormat(format, arg0, arg1, arg2);
		}

		public void Fatal(object message)
		{
			log.Fatal(message);
		}

		public void Fatal(object message, Exception exception)
		{
			log.Fatal(message, exception);
		}

		public void FatalFormat(string format, object arg0)
		{
			log.FatalFormat(format, arg0);
		}

		public void FatalFormat(string format, params object[] args)
		{
			log.FatalFormat(format, args);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			log.FatalFormat(provider, format, args);
		}

		public void FatalFormat(string format, object arg0, object arg1)
		{
			log.FatalFormat(format, arg0, arg1);
		}

		public void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			log.FatalFormat(format, arg0, arg1, arg2);
		}

		public void Info(object message)
		{
			log.Info(message);
		}

		public void Info(object message, Exception exception)
		{
			log.Info(message, exception);
		}

		public void InfoFormat(string format, object arg0)
		{
			log.InfoFormat(format, arg0);
		}

		public void InfoFormat(string format, params object[] args)
		{
			log.InfoFormat(format, args);
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			log.InfoFormat(provider, format, args);
		}

		public void InfoFormat(string format, object arg0, object arg1)
		{
			log.InfoFormat(format, arg0, arg1);
		}

		public void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			log.InfoFormat(format, arg0, arg1, arg2);
		}

		public void Warn(object message)
		{
			log.Warn(message);
		}

		public void Warn(object message, Exception exception)
		{
			log.Warn(message, exception);
		}

		public void WarnFormat(string format, object arg0)
		{
			log.WarnFormat(format, arg0);
		}

		public void WarnFormat(string format, params object[] args)
		{
			log.WarnFormat(format, args);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			log.WarnFormat(provider, format, args);
		}

		public void WarnFormat(string format, object arg0, object arg1)
		{
			log.WarnFormat(format, arg0, arg1);
		}

		public void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			log.WarnFormat(format, arg0, arg1, arg2);
		}

		#endregion

    }
}
