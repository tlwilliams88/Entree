using System.Collections.Generic;

using Entree.Core.Models.Lists.Notes;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface INotesDetailsListRepository {
        NotesListDetail Get(long parentHeaderId, string itemNumber);
        List<NotesListDetail> GetAll(long parentHeaderId);
        long Save(NotesListDetail detail);
    }
}