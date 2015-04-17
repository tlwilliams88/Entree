using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Models.PowerMenu;

namespace KeithLink.Svc.Core.Models.SingleSignOn {
    [DataContract(Name = "powermenucustomeraccessrequests")]
    public class PowerMenuCustomerAccessRequest : BaseAccessRequest {
        #region constructor
        
        public PowerMenuCustomerAccessRequest() {
            RequestType = Enumerations.SingleSignOn.AccessRequestType.PowerMenu;
            
        }
        
        #endregion

        #region properties

        public string Username { get; set; }

        #endregion
    }
}
