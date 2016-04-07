using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface INoteLogic {
        List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo);
    }
}
