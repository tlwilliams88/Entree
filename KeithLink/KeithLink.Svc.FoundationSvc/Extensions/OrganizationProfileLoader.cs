using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationProfileLoader : ProfileLoaderBase
    {
       protected override string ProfileModelName
        {
            get
            {
                return "Organization";
            }
        }

        public OrganizationProfileLoader()
        {
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            base.ExecuteQuery(queryOperation, operationCache, response);
        }
    }
}