using KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.ContentManagement {
    public interface IContentManagementExternalRepository {
        List<ContentItem> GetAllContent();
    }
}
