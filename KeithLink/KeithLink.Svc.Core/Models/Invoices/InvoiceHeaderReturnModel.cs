using KeithLink.Svc.Core.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
	[DataContract]
	public class InvoiceHeaderReturnModel
	{
		[DataMember(Name = "haspayableinvoices")]
		public bool HasPayableInvoices { get; set; }
		[DataMember(Name = "pagedresults")]
		public PagedResults<InvoiceModel> PagedResults { get; set; }
	}
}
