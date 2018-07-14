using Entree.Core.Models.Lists;
using Entree.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace Entree.Core.Interface.Lists {
    public interface IHistoryLogic {
        List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers);
    }
}
