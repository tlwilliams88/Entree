using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface INotesListLogic
    {
        ListItemModel GetNote(UserSelectedContext catalogInfo, string itemNumber);

        List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo);

        ListModel GetList(UserSelectedContext catalogInfo);

        long SaveNote(UserSelectedContext catalogInfo, ListItemModel detail);
    }
}
