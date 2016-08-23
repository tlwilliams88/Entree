using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Core.Exceptions.Queue {
    public class QueueDataError : System.Exception {
        #region constructor
        public QueueDataError(string data, int lineNumber, string method, string process, string message, Exception ex) : base(message, ex) {
            this.Data = data;
            this.LineNumber = lineNumber;
            this.Method = method;
            this.Process = process;
        }
        #endregion

        #region properties
        public override string Message {
            get
            {
                StringBuilder m = new StringBuilder();

                m.AppendLine(String.Format("Process: {0}", Process));
                m.AppendLine(String.Format("Method:{0}", Method));
                m.AppendLine(String.Format("Line Number: {0}", LineNumber));
                m.AppendLine(String.Format("Data: {0}", Data));
                m.AppendLine(String.Format("Exception Message: {0}", base.Message));

                return m.ToString();
            }
        }

        public string Data { get; set; }
        public int LineNumber { get; set; }
        public string Method { get; set; }
        public string Process { get; set; }
        #endregion

    }
}
