using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationAddressesResponseBuilder : CommerceServer.Foundation.SequenceComponents.RelatedProfileResponseBuilderBase
    {
        protected override string CommercePreferredIdKeyName
        {
            get
            {
                return "GeneralInfo.preferred_address";
            }
        }

        protected override string CommerceRelatedIdsKeyName
        {
            get
            {
                return "GeneralInfo.address_list";
            }
        }

        protected override string ParentProfileModelName
        {
            get
            {
                return "Organization";
            }
        }

        protected override string PreferredItemRelationshipName
        {
            get
            {
                return "PreferredAddress";
            }
        }

        protected override string ProfileModelName
        {
            get
            {
                return "Address";
            }
        }

        protected override string RelationshipName
        {
            get
            {
                return "Addresses";
            }
        }

        public override void ExecuteQuery(CommerceServer.Foundation.CommerceQueryOperation queryOperation, CommerceServer.Foundation.OperationCacheDictionary operationCache, CommerceServer.Foundation.CommerceQueryOperationResponse response)
        {
            // don't need to do anything; the profile loading process loads Commerce Entities directly...
        }

        public OrganizationAddressesResponseBuilder()
        {
        }
    }
}