using CommerceServer.Core.Catalog;
using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl
{
    public class CatalogInternalRepositoryImpl: ICatalogInternalRepository
    {
        public void ImportXML(CommerceServer.Core.Catalog.CatalogImportOptions options, System.IO.Stream xmlStream)
        {
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent();
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            CatalogContext context = CatalogContext.Create(catalogSiteAgent);
            ImportProgress importProgress = context.ImportXml(options, xmlStream);
            while (importProgress.Status == CatalogOperationsStatus.InProgress)
            {
                System.Threading.Thread.Sleep(3000);
                importProgress.Refresh();
            }
        }
    }
}
