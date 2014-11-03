using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.Confirmations
{
    [DataContract(Name = "ConfirmationFile")]
    public class ConfirmationFile
    {
        #region constructor

        public ConfirmationFile()
        {
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
