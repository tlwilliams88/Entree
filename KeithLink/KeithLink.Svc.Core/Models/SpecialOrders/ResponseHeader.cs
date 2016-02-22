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
        [DataMember(Name = "RequestHeaderId")]
        public string RequestHeaderId { get; set; }

        [DataMember(Name = "BranchId")]
		public string BranchId { get; set; }

        [DataMember(Name = "BuyerNumber")]
        public string BuyerNumber { get; set; }

        [DataMember(Name = "DsrNumber")]
        public string DsrNumber { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "State")]
        public string State { get; set; }

        [DataMember(Name = "Zip")]
        public string Zip { get; set; }

        [DataMember(Name = "Contact")]
        public string Contact { get; set; }

        [DataMember(Name = "ManufacturerName")]
        public string ManufacturerName { get; set; }

        [DataMember(Name = "ShipMethodId")]
        public byte ShipMethodId { get; set; }

        [DataMember(Name = "OrderStatusId")]
        public string OrderStatusId { get; set; }

        [DataMember(Name = "SpecialInstructions")]
        public string SpecialInstructions { get; set; }

        [DataMember(Name = "ModifiedShippingAddress")]
        public bool? ModifiedShippingAddress { get; set; }

        [DataMember(Name = "CreditApproval")]
        public bool? CreditApproval { get; set; }

        [DataMember(Name = "StatusDate")]
        public string StatusDate { get; set; }

        [DataMember(Name = "SubmitDate")]
        public string SubmitDate { get; set; }

        [DataMember(Name = "UpdatedBy")]
        public string UpdatedBy { get; set; }

        [DataMember(Name = "Source")]

        public string Source { get; set; }
    }
}
