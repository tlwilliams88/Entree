using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Configurations
{
	public class ExportSettingRepositoryImpl: EFBaseRepository<ExportSetting>, IExportSettingRepository
	{
		public ExportSettingRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
	}
}
