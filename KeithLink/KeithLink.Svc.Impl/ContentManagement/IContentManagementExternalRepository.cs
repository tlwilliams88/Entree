using Entree.Core.Models.ContentManagement.ExpressEngine;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.ContentManagement {
    public interface IContentManagementExternalRepository {
        List<ContentItem> GetAllContent();
    }
}
