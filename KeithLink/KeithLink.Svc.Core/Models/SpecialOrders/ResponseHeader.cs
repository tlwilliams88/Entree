using KeithLink.Svc.Core.Interface.ModelExport;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace KeithLink.Svc.Core.Models.SpecialOrders
{
    [DataContract]
    public class ResponseHeader
	{
        [DataMember(Name = "requestheaderid")]
        public string RequestHeaderId { get; set; }

        [DataMember(Name = "branch")]
		public string BranchId { get; set; }

        [DataMember(Name = "buyernumber")]
        public string BuyerNumber { get; set; }

        [DataMember(Name = "dsrnumber")]
        public string DsrNumber { get; set; }

        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "zip")]
        public string Zip { get; set; }

        [DataMember(Name = "contact")]
        public string Contact { get; set; }

        [DataMember(Name = "manufacturername")]
        public string ManufacturerName { get; set; }

        [DataMember(Name = "shipmethod")]
        public byte ShipMethodId { get; set; } 

        [DataMember(Name = "orderstatusid")]
        public string OrderStatusId { get; set; }

        [DataMember(Name = "specialinstructions")]
        public string SpecialInstructions { get; set; }

        [DataMember(Name = "modifiedshippingaddress")]
        public bool? ModifiedShippingAddress { get; set; }

        [DataMember(Name = "creditapproval")]
        public bool? CreditApproval { get; set; }

        [DataMember(Name = "statusdate")]
        public DateTime? StatusDate { get; set; }

        [DataMember(Name = "submitdate")]
        public DateTime? SubmitDate { get; set; }

        [DataMember(Name = "updatedby")]
        public string UpdatedBy { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }
    }
}
