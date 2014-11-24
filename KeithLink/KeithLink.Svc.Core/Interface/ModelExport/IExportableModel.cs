using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ModelExport
{
	public interface IExportableModel
	{
		List<ExportModelConfiguration> DefaultExportConfiguration();
			
	}
}
