using KeithLink.Svc.Core.Models.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ContentManagement
{
	public interface IContentManagementServiceRepository
	{
		void CreateContentItem(ContentItemPostModel item);
        List<ContentItemViewModel> ReadActiveContentItemsByBranch(string branchId, int count);
        ContentItemViewModel ReadContentItemById(int itemId);
        void DeleteContentItemById(int itemId);
	}
}
