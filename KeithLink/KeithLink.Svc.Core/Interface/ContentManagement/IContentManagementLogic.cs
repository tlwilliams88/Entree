using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ContentManagement {
    public interface IContentManagementLogic {
        List<ContentItemViewModel> ReadContentForBranch(string branchId);
    }
}
