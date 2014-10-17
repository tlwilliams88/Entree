using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation;
using CommerceServer.Core.Runtime.Profiles;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationResponseBuilder : ProfileResponseBuilderBase
    {
        protected override string ProfileModelName
        {
            get
            {
                return "Organization";
            }
        }

        public OrganizationResponseBuilder()
        {
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            CommerceModelSearch search = ((CommerceServer.Foundation.CommerceModelSearch)(queryOperation.SearchCriteria));
            if (search.Model.Properties.Count == 1 && search.Model.Properties[0].Key == "OrganizationType")
            {
            }
            else
            {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }

        protected override List<CommerceEntity> TranslateAll(IEnumerable<Profile> commerceProfileList, CommerceEntity userOrganizationModel)
        {
            List<CommerceEntity> commerceEntities = base.TranslateAll(commerceProfileList, userOrganizationModel);
            return commerceEntities;
        }
    }
}