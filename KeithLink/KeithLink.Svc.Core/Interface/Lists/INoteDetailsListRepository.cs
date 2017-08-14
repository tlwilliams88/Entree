using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface INotesDetailsListRepository {
        NotesListDetail Get(long parentHeaderId, string itemNumber);
        List<NotesListDetail> GetAll(long parentHeaderId);
        long Save(NotesListDetail detail);
    }
}