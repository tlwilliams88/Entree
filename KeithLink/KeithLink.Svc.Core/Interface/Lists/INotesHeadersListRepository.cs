using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface INotesHeadersListRepository {
        NotesListHeader GetHeadersForCustomer(UserSelectedContext catalogInfo);
        long Save(NotesListHeader header);
    }
}