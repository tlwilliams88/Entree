using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class OrganizationAddressesResponseBuilder : RelatedProfileResponseBuilderBase {
        #region attributes
        #endregion

        #region ctor
        public OrganizationAddressesResponseBuilder() {
        }
        #endregion

        #region methods
        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            // don't need to do anything; the profile loading process loads Commerce Entities directly...
        }
        #endregion

        #region properties
        protected override string CommercePreferredIdKeyName {
            get {
                return "GeneralInfo.preferred_address";
            }
        }

        protected override string CommerceRelatedIdsKeyName {
            get {
                return "GeneralInfo.address_list";
            }
        }

        protected override string ParentProfileModelName {
            get {
                return "Organization";
            }
        }

        protected override string PreferredItemRelationshipName {
            get {
                return "PreferredAddress";
            }
        }

        protected override string ProfileModelName {
            get {
                return "Address";
            }
        }

        protected override string RelationshipName {
            get {
                return "Addresses";
            }
        }
        #endregion
        
    }
}