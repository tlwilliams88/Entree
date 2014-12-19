using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Core.Interface.Reports
{
	public interface IInternalReportLogic
	{
        List<ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery);
	}
}
