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
	public class ExportSettingServiceRepositoryImpl: IExportSettingServiceRepository
	{
		
        private com.benekeith.ConfigurationService.IConfigurationService serviceClient;
       
		public ExportSettingServiceRepositoryImpl(com.benekeith.ConfigurationService.IConfigurationService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId)
		{
			return serviceClient.ReadCustomExportOptions(userId, type, ListId);
		}

		public void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat)
		{
			serviceClient.SaveUserExportSettings(userId, type, listType, configuration.ToArray(), exportFormat);
		}
	}
}