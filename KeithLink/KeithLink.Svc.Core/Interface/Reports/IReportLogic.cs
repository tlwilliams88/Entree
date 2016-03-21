using KeithLink.Svc.Core.Models.Reports;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Reports {
    public interface IReportLogic {
        List<ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery);
    }
}
