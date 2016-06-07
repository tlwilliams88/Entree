using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Configurations {
    public interface IExportSettingLogic {
        ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId);

        void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat);

        List<ExportExternalCatalog> ReadExternalCatalogs();
    }
}
