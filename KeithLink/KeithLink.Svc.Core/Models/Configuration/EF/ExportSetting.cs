using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Configuration.EF
{
	public enum ExportType
	{
		List,
		Invoice,
		Order,
		Products,
		OrderDetail,
		InvoiceDetail
	}

	public class ExportSetting: BaseEFModel
	{
		public Guid UserId { get; set; }
		public ExportType Type { get; set; }
		public ListType? ListType { get; set; } //Only set then Type == ExportType.List
		public string Settings { get; set; }
		public string ExportFormat { get; set; }
	}
}
