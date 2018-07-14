using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Enumerations.List;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Extensions {

    public static class ListModelExtensions
    {
        public static ListModel SetUserSpecificProperties(this ListModel list, UserProfile user) {
            if (list != null &&
                list.Type != null &&
                user != null &&
                list.Type == ListType.Mandatory && 
                (user != null) &&
                ((string.Compare(user.RoleName, Core.Constants.ROLE_NAME_SYSADMIN, true) == 0) |
                 (string.Compare(user.RoleName, Core.Constants.ROLE_NAME_DSR, true) == 0))) {
                list.ReadOnly = false;
            }
            return list;
        }
    }
}
