using KeithLink.Svc.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.Confirmations
{
    [DataContract(Name = "ConfirmationFile")]
    public class ConfirmationFile : BaseQueueMessage {
        #region constructor
        public ConfirmationFile() {
            Header = new ConfirmationHeader();
            Detail = new List<ConfirmationDetail>();
        }

        #endregion

        #region properties
        [DataMember(Name = "ConfirmationHeader")]
        public ConfirmationHeader Header { get; set; }

        [DataMember(Name = "ConfirmationDetail")]
        public List<ConfirmationDetail> Detail { get; set; }
        #endregion
    }
}
