using Entree.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Reports
{
	public interface IInventoryValuationReportLogic
	{
		MemoryStream GenerateReport(InventoryValuationRequestModel request);
	}
}
