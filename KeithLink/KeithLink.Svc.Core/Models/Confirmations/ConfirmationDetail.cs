using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using KeithLink.Svc.Core;



namespace KeithLink.Svc.Core.Models.Confirmations
{
    [DataContract(Name = "ConfirmationDetail")]
    public class ConfirmationDetail
    {
        [DataMember(Name = "RecordNumber")]
        public string RecordNumber { get; set; }

        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "QuantityOrdered")]
        public int QuantityOrdered { get; set; }

        [DataMember(Name = "BrokenCase")]
        public string BrokenCase { get; set; }

        [DataMember(Name = "QuantityShipped")]
        public int QuantityShipped { get; set; }

        [DataMember(Name = "ReasonNotShipped")]
        public string ReasonNotShipped { get; set; }

        [DataMember(Name = "ShipWeight")]
        public double ShipWeight { get; set; }

        [DataMember(Name = "CaseCube")]
        public double CaseCube { get; set; }

        [DataMember(Name = "CaseWeight")]
        public double CaseWeight { get; set; }

        [DataMember(Name = "SalesGross")]
        public double SalesGross { get; set; }

        [DataMember(Name = "SalesNet")]
        public double SalesNet { get; set; }

        [DataMember(Name = "PriceNet")]
        public double PriceNet { get; set; }

        [DataMember(Name = "SplitPriceNet")]
        public double SplitPriceNet { get; set; }

        [DataMember(Name = "PriceGross")]
        public double PriceGross { get; set; }

        [DataMember(Name = "SplitPriceGross")]
        public double SplitPriceGross { get; set; }

        [DataMember(Name = "ConfirmationMessage")]
        public string ConfirmationMessage { get; set; }
    }
}
