using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IDivisionLogic
    {
		List<Division> GetDivisions();

        List<BranchSupportModel> ReadBranchSupport();
    }
}
