using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface INotesDetailsListRepository {
        List<NotesListDetail> Get(long parentHeaderId);
        long Save(NotesListDetail detail);
    }
}