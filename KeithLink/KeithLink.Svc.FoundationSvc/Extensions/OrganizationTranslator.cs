using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents.Translators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class OrganizationTranslator : ProfileTranslatorBase, IToCommerceEntityTranslator, IToExternalEntityTranslator {
        #region attributes
        #endregion

        #region ctor
        public OrganizationTranslator() {
        }
        #endregion

        #region methods
        public void Translate(object sourceObject, CommerceEntity destinationCommerceEntity, CommercePropertyCollection propertiesToReturn) {
            ParameterChecker.CheckForNull(sourceObject, "sourceObject");
            ParameterChecker.CheckForNull(destinationCommerceEntity, "destinationCommerceEntity");
            base.TranslateInternal(sourceObject as Profile, destinationCommerceEntity, propertiesToReturn);
        }

        public void Translate(CommerceEntity sourceCommerceEntity, object destinationObject) {
            ParameterChecker.CheckForNull(sourceCommerceEntity, "sourceCommerceEntity");
            ParameterChecker.CheckForNull(destinationObject, "destinationObject");
            base.TranslateInternal(sourceCommerceEntity, destinationObject as Profile);
        }
        #endregion

        #region propert
        protected override string ProfileModelName {
            get {
                return "Organization";
            }
        }
        #endregion
    }
}