using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation.SequenceComponents.Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class OrganizationCommitter : ProfileCommitterBase {
        #region attributes
        #endregion

        #region ctor
        public OrganizationCommitter() {
        }
        #endregion

        #region methods
        public override void ExecuteCreate(CommerceCreateOperation createOperation, OperationCacheDictionary operationCache, CommerceCreateOperationResponse response) {
            base.ExecuteCreate(createOperation, operationCache, response);
        }
        #endregion

        #region properties
        protected override string ProfileModelName {
            get { return "Organization"; }
        }
        #endregion


    }
}
