using KeithLink.Svc.Core.Models.ModelExport;
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
	public class InvoiceExportRequestModel
	{
		[DataMember(Name = "paging")]
		public PagingModel paging { get; set; }
		[DataMember(Name = "export")]
		public ExportRequestModel export { get; set; }
	}
}
