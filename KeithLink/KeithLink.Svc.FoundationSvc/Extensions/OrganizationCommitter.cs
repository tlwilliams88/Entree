using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation.SequenceComponents.Utility;
using CommerceServer.Core.Runtime.Profiles;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationCommitter : ProfileCommitterBase
    {
        protected override string ProfileModelName
        {
            get { return "Organization"; }
        }

        public OrganizationCommitter()
        {
        }

        public override void ExecuteCreate(CommerceServer.Foundation.CommerceCreateOperation createOperation, CommerceServer.Foundation.OperationCacheDictionary operationCache, CommerceServer.Foundation.CommerceCreateOperationResponse response)
        {
            base.ExecuteCreate(createOperation, operationCache, response);
        }
    }
}
