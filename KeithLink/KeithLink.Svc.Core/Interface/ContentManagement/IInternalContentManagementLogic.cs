using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ContentManagement;

namespace KeithLink.Svc.Core.Interface.ContentManagement
{
    public interface IInternalContentManagementLogic
    {
        void CreateContentItem(ContentItemPostModel contentItem);
        List<ContentItemViewModel> ReadActiveContentItemsByBranch(string branchId, int count);
        List<ContentItemViewModel> ReadContentItemsByBranch(string branchId, int count);
        void DeleteContentItemById(int itemId);
        ContentItemViewModel ReadContentItemById(int itemId);
    }
}
