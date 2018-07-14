using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace Entree.Core.Interface.Lists {
    public interface INoteLogic {
        List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo);
    }
}
