using Entree.Core.Models.Reports;

using System.Collections.Generic;

namespace Entree.Core.Interface.Reports {
    public interface IReportLogic {
        List<ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery);
    }
}
