﻿using System;
using System.Text;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Order;

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
        [DataMember(Name = "DsrNumber")]
        public string DsrNumber { get; set; }
        [DataMember(Name = "AddressStreet")]
        public string AddressStreet { get; set; }
        [DataMember(Name = "AddressCity")]
        public string AddressCity { get; set; }
        [DataMember(Name = "AddressRegionCode")]
        public string AddressRegionCode { get; set; }
        [DataMember(Name = "AddressPostalCode")]
        public string AddressPostalCode { get; set; }

        [DataMember(Name = "DeliveryDate")]
        public string DeliveryDate { get; set; }

        [DataMember(Name = "PONumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "Specialinstructions")]
        public string Specialinstructions { get; set; }

        [DataMember(Name = "ControlNumber")]
        public int ControlNumber { get; set; }

        [DataMember(Name = "OrderType")]
        public OrderType OrderType { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "OrderCreateDateTime")]
        public DateTime OrderCreateDateTime { get; set; }

        [DataMember(Name = "OrderSendDateTime")]
        public string OrderSendDateTime { get; set; }

        [DataMember(Name = "UserId")]
        public string UserId { get; set; }

        [DataMember(Name = "OrderFilled")]
        public bool OrderFilled { get; set; }

        [DataMember(Name = "FutureOrder")]
        public bool FutureOrder { get; set; }

        [DataMember(Name = "CatalogType")]
        public string CatalogType { get; set; }
        #endregion
    }
}
