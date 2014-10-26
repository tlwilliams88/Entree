using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation.SequenceComponents.Translators;
using CommerceServer.Foundation;
using CommerceServer.Core.Runtime.Profiles;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class UserOrganizationsTranslator : ProfileTranslatorBase, IToCommerceEntityTranslator, IToExternalEntityTranslator
    {
        protected override string ProfileModelName
        {
            get
            {
                return "UserOrganizations";
            }
        }

        public UserOrganizationsTranslator()
        {
        }

        public void Translate(object sourceObject, CommerceEntity destinationCommerceEntity, CommercePropertyCollection propertiesToReturn)
        {
            ParameterChecker.CheckForNull(sourceObject, "sourceObject");
            ParameterChecker.CheckForNull(destinationCommerceEntity, "destinationCommerceEntity");
            base.TranslateInternal(sourceObject as Profile, destinationCommerceEntity, propertiesToReturn);
        }

        public void Translate(CommerceEntity sourceCommerceEntity, object destinationObject)
        {
            ParameterChecker.CheckForNull(sourceCommerceEntity, "sourceCommerceEntity");
            ParameterChecker.CheckForNull(destinationObject, "destinationObject");
            base.TranslateInternal(sourceCommerceEntity, destinationObject as Profile);
        }
    }
}