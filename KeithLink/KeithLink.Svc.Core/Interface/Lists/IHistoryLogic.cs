using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IHistoryLogic {
        List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers);
    }
}
