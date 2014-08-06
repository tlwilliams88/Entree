using CommerceServer.Core.Catalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface ICatalogInternalRepository
    {
        void ImportXML(CatalogImportOptions options, Stream xmlStream);
    }
}
