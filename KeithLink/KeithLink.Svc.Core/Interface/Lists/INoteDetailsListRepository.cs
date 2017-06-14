using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface INotesDetailsListRepository {
        NotesListDetail Get(UserSelectedContext catalogInfo);
        long Save(NotesListDetail detail);
    }
}