using System.Collections.Generic;

using Entree.Core.Models.Lists.Notes;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface INotesHeadersListRepository {
        NotesListHeader GetHeadersForCustomer(UserSelectedContext catalogInfo);
        long Save(NotesListHeader header);
    }
}