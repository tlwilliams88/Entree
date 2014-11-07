using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Core.Models.Orders.Confirmations
{
    [DataContract(Name = "ConfirmationHeader")]
    public class ConfirmationHeader
    {
        #region properties
        [DataMember(Name = "ConfirmationDate")]
        public DateTime ConfirmationDate { get; set; }

        [DataMember(Name = "Branch")]
        public string Branch { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "UserId")]
        public string UserId { get; set; }

        [DataMember(Name = "Remote_OrderNumber")]
        public string RemoteOrderNumber { get; set; }

        [DataMember(Name = "ConfirmationNumber")]
        public string ConfirmationNumber { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "ShipDate")]
        public DateTime? ShipDate { get; set; }

        [DataMember(Name = "RouteNumber")]
        public string RouteNumber { get; set; }

        [DataMember(Name = "StopNumber")]
        public string StopNumber { get; set; }

        [DataMember(Name = "SpecialInstructions")]
        public string SpecialInstructions { get; set; }

        [DataMember(Name = "SpecialInstructionsExtended")]
        public string SpecialInstructionsExtended { get; set; }

        [DataMember(Name = "TotalQuantityOrdered")]
        public int TotalQuantityOrdered { get; set; }

        [DataMember(Name = "TotalQuantityShipped")]
        public int TotalQuantityShipped { get; set; }

        [DataMember(Name = "TotalInvoice")]
        public double TotalInvoice { get; set; }

        [DataMember(Name = "TotalCube")]
        public double TotalCube { get; set; }

        [DataMember(Name = "TotalWeight")]
        public double TotalWeight { get; set; }

        [DataMember(Name = "ConfirmationMessage")]
        public string ConfirmationMessage { get; set; }

        [DataMember(Name = "ConfirmationStatus")]
        public string ConfirmationStatus { get; set; }

        // TODO: Finish this after looking at the mock screens.

        #endregion
    }
}
