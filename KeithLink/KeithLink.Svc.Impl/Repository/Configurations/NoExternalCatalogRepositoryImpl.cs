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
	public class NoExternalCatalogRepositoryImpl: IExternalCatalogServiceRepository {
        #region attributes
        #endregion

        #region ctor
        public NoExternalCatalogRepositoryImpl()
		{
		}
        #endregion


        public List<ExportExternalCatalog> ReadExternalCatalogs()
        {
            return new List<ExportExternalCatalog>();
        }
    }
}
