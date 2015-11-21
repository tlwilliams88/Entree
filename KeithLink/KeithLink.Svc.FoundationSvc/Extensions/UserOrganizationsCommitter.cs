using CommerceServer.Foundation.SequenceComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class UserOrganizationsCommitter : ProfileCommitterBase {
        #region attributes
        #endregion

        #region ctor
        public UserOrganizationsCommitter() {
        }
        #endregion

        #region methods
        #endregion

        #region properties
        protected override string ProfileModelName {
            get { return "UserOrganizations"; }
        }
        #endregion
    }
}
