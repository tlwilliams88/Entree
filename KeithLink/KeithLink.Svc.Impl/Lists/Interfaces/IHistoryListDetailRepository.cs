using System.Collections.Generic;

using Entree.Core.Models.Lists.History;

namespace Entree.Core.Interface.Lists {
    public interface IHistoryListDetailRepository {
        List<HistoryListDetail> GetAllHistoryDetails(long listId);
    }
}
