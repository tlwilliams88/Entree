using CommerceServer.Core.Catalog;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeithLink.Svc.Impl.Repository.InternalCatalog
{
    public class CatalogInternalRepositoryImpl: ICatalogInternalRepository
    {
        public void ImportXML(CommerceServer.Core.Catalog.CatalogImportOptions options, System.IO.Stream xmlStream)
        {
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent();
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            CatalogContext context = CatalogContext.Create(catalogSiteAgent);
            ImportProgress importProgress = context.ImportXml(options, xmlStream);

            try
            {
                while (importProgress.Status == CatalogOperationsStatus.InProgress)
                {
                    System.Threading.Thread.Sleep(3000);

                    importProgress.Refresh();
                }
            }
            catch (Exception ex)
            {
                if (importProgress.Status == CatalogOperationsStatus.Failed)
                {
                    StringBuilder errors = new StringBuilder();

                    foreach (CatalogError e in importProgress.Errors)
                    {
                        errors.AppendLine(String.Format("Line: {0} - Message: {1}", e.LineNumber, e.Message));
                    }

                    Exception newException = new Exception(errors.ToString(), ex);

                    throw newException;
                }

            }

        }
    }
}
