using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public enum InvoiceType
	{
		Invoice,
		CreditMemo,
		Adjustment
	}

	public enum InvoiceStatus
	{
		Open,
		Paid
	}

	public class Invoice: BaseEFModel
	{
		public string CustomerNumber { get; set; }
		[MaxLength(3)]
		public string BranchId { get; set; }
		public string InvoiceNumber { get; set; }
		public DateTime? OrderDate { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public InvoiceType Type { get; set; }
		public decimal Amount { get; set; }
		public InvoiceStatus Status { get; set; }


		public virtual ICollection<InvoiceItem> Items { get; set; }
	}
}
