using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.Notes;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace Entree.Core.Interface.Lists
{
    public interface INotesListLogic
    {
        ListItemModel GetNote(UserSelectedContext catalogInfo, string itemNumber);

        List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo);

        ListModel GetList(UserSelectedContext catalogInfo);

        long SaveNote(UserSelectedContext catalogInfo, ListItemModel detail);
    }
}
