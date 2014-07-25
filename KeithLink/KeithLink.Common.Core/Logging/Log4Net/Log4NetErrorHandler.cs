using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using System.IO;

namespace KeithLink.Common.Core.Logging.Log4Net
{
    class Log4NetErrorHandler : IErrorHandler
    {
        private const string FILE_LOG4NET_ERROR_DOCUMENT = "log4net.errors.log";

        private static string ErrorLogFilePath
        {
            get
            {
                try
                {
                    // TODO:  Fix logging location

                    string appRoot = System.Web.HttpContext.Current == null
                        ? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).TrimEnd('\\')
                        : System.Web.HttpContext.Current.Server.MapPath("bin");

                    return string.Format("{0}\\{1}", appRoot, FILE_LOG4NET_ERROR_DOCUMENT);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        #region IErrorHandler Members

        public void Error(string message)
        {
            File.AppendAllText(ErrorLogFilePath, message);
        }

        public void Error(string message, Exception e)
        {
            File.AppendAllText(ErrorLogFilePath, string.Format("{0} :: {1}\r\n{2}",
                message
                , e.Message
                , e.StackTrace));
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            File.AppendAllText(ErrorLogFilePath, string.Format("{0} - {1} :: {2}\r\n{3}",
                message
                , errorCode
                , e.Message
                , e.StackTrace));
        }

        #endregion
    }
}
