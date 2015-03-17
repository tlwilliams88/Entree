using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.SingleSignOn {
    public interface IKbitRequestLogic {
        void UpdateUserAccess(string userName, List<UserSelectedContext> customers);
    }
}
