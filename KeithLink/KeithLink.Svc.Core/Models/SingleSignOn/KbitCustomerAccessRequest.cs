using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SingleSignOn {
    [DataContract(Name="kbitcustomeraccessrequest")]
    public class KbitCustomerAccessRequest : BaseAccessRequest {
        #region ctor
        public KbitCustomerAccessRequest() {
            RequestType = Enumerations.SingleSignOn.AccessRequestType.KbitCustomer;

            Customers = new List<UserSelectedContext>();
        }
        #endregion

        #region properties
        [DataMember(Name="customers")]
        public List<UserSelectedContext> Customers { get; set; }
        #endregion
    }
}
