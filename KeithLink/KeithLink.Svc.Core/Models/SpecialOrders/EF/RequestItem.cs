using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.SpecialOrders.EF {
    public class RequestItem {
        [Key, Column(Order = 0), Required]
		public string BranchId { get; set; } // CHAR(3),
        [Key, Column(Order=1), Required]
		public string RequestHeaderId  { get; set; } // CHAR(7),
        [Key, Column(Order = 2), Required]
		public byte LineNumber { get; set; } // SMALLINT,
        [Key, Column(Order = 3), Required]
		public string OrderStatusId  { get; set; } // CHAR(2),
		public string ManufacturerNumber  { get; set; } // VARCHAR(20) = NULL,
		public string GtinUpc	 { get; set; } // VARCHAR(14) = NULL,
		public string Description  { get; set; } // VARCHAR(100) = NULL,
		public Int16? Quantity { get; set; } //SMALLINT = NULL,
		public string UnitOfMeasure	 { get; set; } // VARCHAR(20) = NULL,
		public string BekItemNumber	 { get; set; } // CHAR(6) = NULL,
		public string BekInvoiceNumber  { get; set; } // CHAR(8) = NULL,
		public float? EstimateCost { get; set; } // MONEY = NULL,
		public float? Price  { get; set; } // MONEY = NULL,
		public decimal? EsitamtedGPPercent  { get; set; } // DECIMAL(5,2) = NULL,
		public string Comments { get; set; } // VARCHAR(500) = NULL,
		public string PONumber  { get; set; } // CHAR(6) = NULL,
		public DateTime? EstimatedArrival { get; set; } // DATETIME = NULL,
		public string ArrivalDateFlag  { get; set; } // CHAR(1) = NULL,
		public string UpdatedBy  { get; set; } // VARCHAR(50) = NULL,
		public byte ShipMethodId { get; set; } // TINYINT

		/*
        [Key, Column("Division", TypeName="char", Order=1), MaxLength(5)]
		public string Division { get; set; }

        [Key, Column(TypeName="char", Order = 2), MaxLength(6)]
        public string CustomerNumber { get; set; }

        [Key, Column(Order=3), MaxLength(30)]
        public string InvoiceNumber { get; set; }

        [Key, Column(Order=4), MaxLength(17)]
        public string AccountNumber { get; set; }

        [Key, Column(Order=5), MaxLength(30)]
        public string UserName { get; set; }

        [Key, Column(Order=6)]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        [Required]
        public int ConfirmationId { get; set; }

        [Column(TypeName="date")]
        public DateTime? ScheduledPaymentDate { get; set; } */
    }
}
