using System;
using System.Collections.Generic;
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
		public string InvoiceNumber { get; set; }
		public InvoiceType Type { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime? ShipDate { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? RouteNumber { get; set; }
		public int? StopNumber { get; set; }
		public DateTime? DateTimeOfLastOrder { get; set; }

		public int CustomerNumber { get; set; }
		public string Division { get; set; }
		public string Company { get; set; }
		public string Department { get; set; }
		public string WHNumber { get; set; }
		public int? OrderNumber { get; set; }
		public string MemoBillCode { get; set; }
		public string CreditHoldFlag { get; set; }
		public string TradeSWFlag { get; set; }
		public string CustomerGroup { get; set; }
		public string SalesRep { get; set; }
		public string ChainStoreCode { get; set; }

		public virtual ICollection<InvoiceItem> Items { get; set; }
	}
}
