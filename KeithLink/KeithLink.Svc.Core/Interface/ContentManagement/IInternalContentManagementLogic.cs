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
    }
}
