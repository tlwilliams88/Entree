using System;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IDivisionLogic
    {
		List<Division> GetDivisions();
	}
}
