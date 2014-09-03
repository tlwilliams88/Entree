using System;
using System.Text;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name="OrderHeader")]
    public class OrderHeader
    {
        #region properties
        [DataMember(Name="OrderingSystem")]
        public OrderSource OrderingSystem { get; set; }

        [DataMember(Name = "Branch")]
        public string Branch { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "DeliveryDate")]
        public DateTime DeliveryDate { get; set; }

        [DataMember(Name = "PONumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "Specialinstructions")]
        public string Specialinstructions { get; set; }

        [DataMember(Name = "ControlNumber")]
        public string ControlNumber { get; set; }

        [DataMember(Name = "OrderType")]
        public OrderType OrderType { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "OrderCreateDateTime")]
        public DateTime OrderCreateDateTime { get; set; }

        [DataMember(Name = "OrderSendDateTime")]
        public DateTime OrderSendDateTime { get; set; }

        [DataMember(Name = "OrderFilled")]
        public bool OrderFilled { get; set; }

        [DataMember(Name = "FutureOrder")]
        public bool FutureOrder { get; set; }
        #endregion
    }
}
