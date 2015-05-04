using KeithLink.Svc.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Reports
{
	public interface IInventoryValuationReportLogic
	{
		MemoryStream GenerateReport(InventoryValuationRequestModel request);
	}
}
