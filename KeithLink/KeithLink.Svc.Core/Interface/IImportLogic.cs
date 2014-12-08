using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface
{
	public interface IImportLogic
	{
		ListImportModel ImportList(UserProfile user, UserSelectedContext catalogInfo, string csvFile);
        OrderImportModel ImportOrder( UserProfile user, UserSelectedContext catalogInfo, OrderImportFileModel file );
	}
}
