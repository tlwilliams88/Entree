using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation;
using CommerceServer.Core.Runtime.Profiles;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class UserOrganizationsResponseBuilder : ProfileResponseBuilderBase
    {
        protected override string ProfileModelName
        {
            get
            {
                return "UserOrganizations";
            }
        }

        public UserOrganizationsResponseBuilder()
        {
        }

        protected override List<CommerceEntity> TranslateAll(IEnumerable<Profile> commerceProfileList, CommerceEntity userOrganizationModel)
        {
            List<CommerceEntity> commerceEntities = base.TranslateAll(commerceProfileList, userOrganizationModel);
            return commerceEntities;
        }
    }
}