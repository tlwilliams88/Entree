using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Export
{
	public interface IModelExportLogic<T> where T : class, IExportableModel
	{
		MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType, UserSelectedContext context);
		MemoryStream Export(IList<T> model, string exportType, UserSelectedContext context);
	}
}
