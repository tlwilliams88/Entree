using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.Configurations
{
	public class ExternalCatalogServiceRepositoryImpl: IExternalCatalogServiceRepository
	{
		
        private com.benekeith.ConfigurationService.IConfigurationService serviceClient;

        public ExternalCatalogServiceRepositoryImpl(com.benekeith.ConfigurationService.IConfigurationService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

        public List<ExternalCatalog> ReadExternalCatalogs()
        {
            throw new NotImplementedException();
        }
    }
}