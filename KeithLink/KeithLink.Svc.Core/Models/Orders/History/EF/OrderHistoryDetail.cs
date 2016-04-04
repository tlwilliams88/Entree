﻿using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders.History.EF
{
	public class OrderHistoryDetail: BaseEFModel
	{
        [MaxLength(3)]
        [Column(TypeName = "char")]
        [Index("IdxOrderDetail", 0)]
        public string BranchId { get; set; }
        [MaxLength(10)]
        [Column(TypeName = "varchar")]
        [Index("IdxOrderDetail", 1)]
        public string InvoiceNumber { get; set; }
        [Index("IdxOrderDetail", 2)]
        public int LineNumber { get; set; }
        [MaxLength(6)]
		[Column(TypeName = "char")]
        [Index("IdxItemUsageGrouping", 0)]
		public string ItemNumber { get; set; }
		public int OrderQuantity { get; set; }
		public int ShippedQuantity { get; set; }
		[MaxLength(1)]
		[Column(TypeName = "char")]
        [Index("IdxItemUsageGrouping", 1)]
		public string UnitOfMeasure { get; set; }
		public bool CatchWeight { get; set; }
		public bool ItemDeleted { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
		public string SubbedOriginalItemNumber { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
		public string ReplacedOriginalItemNumber { get; set; }
		[MaxLength(1)]
		[Column(TypeName = "char")]
		public string ItemStatus { get; set; }
		public decimal TotalShippedWeight { get; set; }
        public decimal SellPrice { get; set; }
        [MaxLength(3)]
        [Column(TypeName = "char")]
        public string Source { get; set; }
        [MaxLength(25)]
        public string ManufacturerId { get; set; }
        [MaxLength(7)]
        [Column(TypeName = "char")]
        public string SpecialOrderHeaderId { get; set; }
        [MaxLength(3)]
        [Column(TypeName = "char")]
        public string SpecialOrderLineNumber { get; set; }

        public virtual OrderHistoryHeader OrderHistoryHeader { get; set; }
	}
}
