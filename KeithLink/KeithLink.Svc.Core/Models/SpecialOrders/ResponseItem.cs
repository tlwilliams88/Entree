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
        [DataMember(Name = "catalog")]
        public string Catalog { get; set; }

        [DataMember(Name = "linenumber")]
        public string LineNumber { get; set; }

        [DataMember(Name = "itemstatusid")]
        public string ItemStatusId { get; set; }

        [DataMember(Name = "manufacturernumber")]
        public string ManufacturerNumber { get; set; }

        [DataMember(Name = "gtinupc")]
        public string GtinUpc { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "quantityrequested")]
        public Int16? QuantityRequested { get; set; }

        [DataMember(Name = "quantityshipped")]
        public Int16? QuantityShipped { get; set; }

        [DataMember(Name = "unitofmeasure")]
        public string UnitOfMeasure { get; set; }

        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "invoicenumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "estimatecost")]
        public float? EstimateCost { get; set; } 

        [DataMember(Name = "price")]
        public float? Price { get; set; }

        [DataMember(Name = "estimatedgppercent")]
        public decimal? EstimatedGPPercent { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }

        [DataMember(Name = "ponumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "estimatedarrival")]
        public DateTime? EstimatedArrival { get; set; }

        [DataMember(Name = "arrivaldateflag")]
        public string ArrivalDateFlag { get; set; }

        [DataMember(Name = "updatedby")]
        public string UpdatedBy { get; set; }

        [DataMember(Name = "shipmethodid")]
        public byte ShipMethodId { get; set; }
    }
}
