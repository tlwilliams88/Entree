using Entree.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.SingleSignOn {
    public interface IKbitRequestLogic {
        void UpdateUserAccess(string userName, List<UserSelectedContext> customers);
    }
}
