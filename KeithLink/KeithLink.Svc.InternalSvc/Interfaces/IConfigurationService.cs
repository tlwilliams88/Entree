using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IExportSettingsService" in both code and config file together.
	[ServiceContract]
	public interface IConfigurationService
	{
		[OperationContract]
		ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId);
		[OperationContract]
		void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat);
	}
}
