using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Common {
    [DataContract]
	public abstract class BaseQueueMessage {
        #region ctor
        public BaseQueueMessage() {
            CreateDateTime = DateTime.Now;
            MessageId = Guid.NewGuid();
            SenderIPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Where(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First().ToString();
            SenderMachineName = System.Environment.MachineName;
        }
        #endregion

        #region properties
        [DataMember(Name = "CreateDateTime")]
        public DateTime CreateDateTime { get; set; }

        [DataMember(Name = "MessageId")]
        public Guid MessageId { get; set; }

        [DataMember(Name = "SenderApplicationName")]
        public string SenderApplicationName { get; set; }

        [DataMember(Name = "SenderIPAddress")]
        public string SenderIPAddress { get; set; }

        [DataMember(Name = "SenderMachineName")]
        public string SenderMachineName { get; set; }

        [DataMember(Name = "SenderProcessName")]
        public string SenderProcessName { get; set; }

        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "ErrorStack")]
        public string ErrorStack { get; set; }
        #endregion
    }
}
