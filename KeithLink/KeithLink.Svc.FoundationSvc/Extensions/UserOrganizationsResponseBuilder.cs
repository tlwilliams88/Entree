using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class UserOrganizationsResponseBuilder : ProfileResponseBuilderBase {
        #region attributes
        #endregion

        #region ctor
        public UserOrganizationsResponseBuilder() {
        }
        #endregion

        #region methods
        protected override List<CommerceEntity> TranslateAll(IEnumerable<Profile> commerceProfileList, CommerceEntity userOrganizationModel) {
            List<CommerceEntity> commerceEntities = base.TranslateAll(commerceProfileList, userOrganizationModel);
            return commerceEntities;
        }
        #endregion

        #region properties
        protected override string ProfileModelName {
            get {
                return "UserOrganizations";
            }
        }
        #endregion


    }
}