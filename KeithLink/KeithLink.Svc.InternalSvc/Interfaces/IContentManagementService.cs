using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ContentManagement;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    interface IContentManagementService
    {
        void CreateContentItem(ContentItemPostModel contentItem);
    }
}
