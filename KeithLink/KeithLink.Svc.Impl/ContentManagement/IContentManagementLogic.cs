using Entree.Core.Interface.ContentManagement;
using Entree.Core.Models.ContentManagement;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.ContentManagement {
    public interface IContentManagementLogic {
        List<ContentItemViewModel> ReadContentForBranch(string branchId);
        bool LogHit(UserProfile user, UserSelectedContext context, ContentItemClickedModel clicked);
    }
}
