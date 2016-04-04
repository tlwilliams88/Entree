using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Configurations
{
	public class InternalExternalCatalogServiceRepositoryImpl: IExternalCatalogServiceRepository {
        #region attributes
        IInternalExportSettingLogic _exportSettingLogic;
        #endregion

        #region ctor
        public InternalExternalCatalogServiceRepositoryImpl(IInternalExportSettingLogic exportSettingLogic)
		{
            _exportSettingLogic = exportSettingLogic;
		}
        #endregion


        public List<ExportExternalCatalog> ReadExternalCatalogs()
        {
            return _exportSettingLogic.ReadExternalCatalogs();
        }
    }
}
