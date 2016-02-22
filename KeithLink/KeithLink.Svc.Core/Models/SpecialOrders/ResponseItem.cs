using KeithLink.Svc.Core.Interface.ModelExport;
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace KeithLink.Svc.Core.Models.SpecialOrders {
    [DataContract]
    public class ResponseItem
    {
        [DataMember(Name = "Catalog")]
        public string Catalog { get; set; }

        [DataMember(Name = "LineNumber")]
        public string LineNumber { get; set; }

        [DataMember(Name = "ItemStatusId")]
        public string ItemStatusId { get; set; }

        [DataMember(Name = "ManufacturerNumber")]
        public string ManufacturerNumber { get; set; }

        [DataMember(Name = "GtinUpc")]
        public string GtinUpc { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "QuantityRequested")]
        public Int16? QuantityRequested { get; set; }

        [DataMember(Name = "QuantityShipped")]
        public Int16? QuantityShipped { get; set; }

        [DataMember(Name = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "EstimateCost")]
        public float? EstimateCost { get; set; }

        [DataMember(Name = "Price")]
        public float? Price { get; set; }

        [DataMember(Name = "EstimatedGPPercent")]
        public decimal? EstimatedGPPercent { get; set; }

        [DataMember(Name = "Comments")]
        public string Comments { get; set; }

        [DataMember(Name = "PONumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "EstimatedArrival")]
        public string EstimatedArrival { get; set; }

        [DataMember(Name = "ArrivalDateFlag")]
        public string ArrivalDateFlag { get; set; }

        [DataMember(Name = "UpdatedBy")]
        public string UpdatedBy { get; set; }

        [DataMember(Name = "ShipMethodId")]
        public byte ShipMethodId { get; set; }
    }
}
