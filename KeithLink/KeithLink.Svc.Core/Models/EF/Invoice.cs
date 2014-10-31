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
		KeithPay,
		KeithNet
	}


	public class Invoice: BaseEFModel
	{
		[MaxLength(10)]
		[Column(TypeName = "char")]
		public string CustomerNumber { get; set; }
		public string InvoiceNumber { get; set; }
		public DateTime? ShipDate { get; set; }
		public DateTime? OrderDate { get; set; }
		

		public virtual ICollection<InvoiceItem> Items { get; set; }
	}
}
