using KeithLink.Svc.Core.Models.ContentManagement.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ContentManagement
{
	public interface IContentManagementItemRepository : IBaseEFREpository<ContentItem>
	{
        IEnumerable<ContentItem> ReadActiveContentItemsByBranch(string branchId, int count);
        IEnumerable<ContentItem> ReadContentItemsByBranch(string branchId, int count);
        string SaveContentImage( long contentId, string fileName, string base64File );
	}
}
