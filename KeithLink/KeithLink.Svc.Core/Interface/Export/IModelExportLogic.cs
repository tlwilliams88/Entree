using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
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
		MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType);
		MemoryStream Export(IList<T> model, string exportType);
	}
}
