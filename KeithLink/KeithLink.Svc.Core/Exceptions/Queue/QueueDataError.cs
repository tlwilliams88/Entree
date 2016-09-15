using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Core.Exceptions.Queue {
    public class QueueDataError<T> : System.Exception {
        #region constructor
        public QueueDataError(T data, string method, string process, string message, Exception ex) : base(message, ex) {
            this.ProcessingObject = data;
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
                m.AppendLine(String.Format("Processing Object: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(ProcessingObject)));
                m.AppendLine(String.Format("Exception Message: {0}", base.Message));

                return m.ToString();
            }
        }

        public T ProcessingObject { get; set; }
        public string Method { get; set; }
        public string Process { get; set; }
        #endregion

    }
}
