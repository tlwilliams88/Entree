using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class OrganizationResponseBuilder : ProfileResponseBuilderBase {
        #region attributes
        #endregion

        #region ctor
        public OrganizationResponseBuilder() {
        }
        #endregion

        #region methods
        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            CommerceModelSearch search = ((CommerceModelSearch)(queryOperation.SearchCriteria));

            if (!String.IsNullOrEmpty(search.WhereClause)) {
                // do nothing
            } else {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }

        protected override List<CommerceEntity> TranslateAll(IEnumerable<Profile> commerceProfileList, CommerceEntity userOrganizationModel) {
            List<CommerceEntity> commerceEntities = base.TranslateAll(commerceProfileList, userOrganizationModel);
            return commerceEntities;
        }
        #endregion

        #region properties
        protected override string ProfileModelName {
            get {
                return "Organization";
            }
        }
        #endregion


    }
}