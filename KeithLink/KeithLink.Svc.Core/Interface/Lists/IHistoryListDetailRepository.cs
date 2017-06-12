using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.History;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IHistoryListDetailRepository
    {
        List<HistoryListDetail> GetAllHistoryDetails(long listId);
    }
}
