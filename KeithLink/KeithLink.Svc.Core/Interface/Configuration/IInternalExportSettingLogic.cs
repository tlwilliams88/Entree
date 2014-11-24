using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Configuration
{
	public interface IInternalExportSettingLogic
	{
		ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId);
		void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat);
	}
}
