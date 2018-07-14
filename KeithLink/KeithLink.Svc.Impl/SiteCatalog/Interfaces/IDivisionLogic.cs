using Entree.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.SiteCatalog
{
    public interface IDivisionLogic
    {
		List<Division> GetDivisions();

        List<BranchSupportModel> ReadBranchSupport();
    }
}
