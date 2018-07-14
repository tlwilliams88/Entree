using Entree.Core.Interface.ModelExport;
using Entree.Core.Models.ModelExport;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Export
{
    public interface IModelExportLogic<T> where T : class, IExportableModel
    {
        MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType, UserSelectedContext context, dynamic headerInfo = null);
        MemoryStream Export(IList<T> model, string exportType, UserSelectedContext context, dynamic headerInfo = null);
    }
}