using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KeithLink.Svc.Core.Models.SpecialOrders.EF
{
	public class RequestHeader
	{
        [Key, Required]
        public string RequestHeaderId { get; set; } // char(7)
		public string BranchId { get; set; } // (char(3)
        public byte CategoryId { get; set; } // TINYINT,
        public string BuyerNumber { get; set; } // CHAR(2) = NULL,
        public string DsrNumber { get; set; } // CHAR(3)
        public string CustomerNumber { get; set; } // CHAR(6),
        public string Address { get; set; } // VARCHAR(25),
        public string City { get; set; } // VARCHAR(18),
        public string State { get; set; } // CHAR(2),
        public string Zip { get; set; } // VARCHAR(9),
        public string Contact { get; set; } // VARCHAR(50) = NULL,
        public string ManufacturerName  { get; set; } // VARCHAR(50),
        public byte ShipMethodId { get; set; } // TINYINT,
        public string OrderStatusId { get; set; } // CHAR(2),
        public string SpecialInstructions { get; set; } // VARCHAR(500) = NULL,
        public bool? ModifiedShippingAddress { get; set; } // BIT = NULL,
        public bool? CreditApproval { get; set; } // BIT = NULL,
        public DateTime? StatusDate { get; set; } // DATETIME = NULL,
        public DateTime? SubmitDate { get; set; } // DATETIME = NULL,
        public string UpdatedBy { get; set; } // VARCHAR(50)
        public string Source { get; set; } //Char(3)
    }
}
