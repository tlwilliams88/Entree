using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IInternalDivisionLogic
    {
		List<BranchSupportModel> ReadBranchSupport();
    }
}
