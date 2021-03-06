﻿using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Orders.History {

    [DataContract(Name="OrderHistoryHeader")]
    public class OrderHistoryHeader {
        #region properties

        [DataMember(Name="ordersystem")]
        public OrderSource OrderSystem { get; set; }

        [DataMember(Name="branchid")]
        public string BranchId { get; set; }

        [DataMember(Name="customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name="invoicenumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "orderdatetime")]
        public string OrderDateTime { get; set; }

        [DataMember(Name="deliverydate")]
        public string DeliveryDate { get; set; }

        [DataMember(Name="ponumber")]
        public string PONumber { get; set; }

        [DataMember(Name="controlnumber")]
        public string ControlNumber { get; set; }

        [DataMember(Name = "originalcontrolnumber")]
        public string OriginalControlNumber { get; set; }

        [DataMember(Name = "orderstatus")]
        public string OrderStatus { get; set; }

        [DataMember(Name="futureitems")]
        public bool FutureItems { get; set; }

        [DataMember(Name="errorstatus")]
        public bool ErrorStatus { get; set; }

        [DataMember(Name="routenumber")]
        public string RouteNumber { get; set; }

        [DataMember(Name="stopnumber")]
        public string StopNumber { get; set; }

        [DataMember(Name = "scheduledtime")]
        public string ScheduledDeliveryTime { get; set; }

        [DataMember(Name = "estimatedtime")]
        public string EstimatedDeliveryTime { get; set; }

        [DataMember(Name = "actualtime")]
        public string ActualDeliveryTime { get; set; }

        [DataMember(Name = "deliveryoutofsequence")]
        public bool? DeliveryOutOfSequence { get; set; }

        [DataMember(Name = "items")]
        public List<OrderHistoryDetail> Items { get; set; }

        #endregion
    }
}
