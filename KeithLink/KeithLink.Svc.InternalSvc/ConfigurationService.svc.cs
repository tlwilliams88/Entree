using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ExportSettingsService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select ExportSettingsService.svc or ExportSettingsService.svc.cs at the Solution Explorer and start debugging.
	public class ConfigurationService : IConfigurationService
	{
		private readonly IInternalExportSettingLogic exportSettingLogic;

		public ConfigurationService(IInternalExportSettingLogic exportSettingLogic)
		{
			this.exportSettingLogic = exportSettingLogic;
		}

		public ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId)
		{
			return exportSettingLogic.ReadCustomExportOptions(userId, type, ListId);
		}

		public void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration)
		{
			exportSettingLogic.SaveUserExportSettings(userId, type, listType, configuration);
		}
	}
}
