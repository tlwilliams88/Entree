using System;

namespace KeithLink.Svc.Core.Exceptions.Queue {
    public class QueueConnectionException : System.Exception {
        #region ctor
        public QueueConnectionException(string serverName, string vhostName, string exchangeName, string queueName, string message, Exception innerException) : base(message, innerException) {
            Server = serverName;
            VHost = vhostName;
            Exchange = exchangeName;
            Queue = queueName;
        }
        #endregion

        #region properties
        public string Exchange { get; set; }

        public override string Message {
            get {
                System.Text.StringBuilder msg = new System.Text.StringBuilder();

                msg.AppendLine("Problems encountered while connecting to Rabbit MQ Server.");
                msg.AppendLine(string.Concat("Server: ", Server));
                msg.AppendLine(string.Concat("VHost: ", VHost));
                msg.AppendLine(string.Concat("Exchange: ", Exchange));
                msg.AppendLine(string.Concat("Queue: ", Queue));
                msg.AppendLine(string.Concat("Inner Exception Message: ", base.Message));

                return msg.ToString();
            }
        }

        public string Queue { get; set; }

        public string Server { get; set; }
        
        public string VHost { get; set; }
        #endregion

    }
}
