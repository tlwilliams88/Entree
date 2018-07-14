using Entree.Core.Enumerations.List;
using Entree.Core.Models.Configuration.EF;
using Entree.Core.Models.ModelExport;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.Configurations {
    public interface IExportSettingLogic {
        ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, ListType listType);

        void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat);

        List<ExportExternalCatalog> ReadExternalCatalogs();
    }
}
