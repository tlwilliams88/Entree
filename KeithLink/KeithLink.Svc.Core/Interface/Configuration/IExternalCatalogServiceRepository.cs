﻿using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Configuration
{
	public interface IExternalCatalogServiceRepository
	{
        List<ExternalCatalog> ReadExternalCatalogs();
	}
}
