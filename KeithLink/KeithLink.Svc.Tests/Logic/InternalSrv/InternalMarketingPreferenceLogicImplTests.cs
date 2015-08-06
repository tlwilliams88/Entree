using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic.InternalSrv {
    [TestClass]
    public class InternalMarketingPreferenceLogicImplTests {
        #region attributes
        private readonly IInternalMarketingPreferenceLogic _logic;
        #endregion

        #region ctor
        public InternalMarketingPreferenceLogicImplTests() {
            IContainer diMap = DependencyMap.Build();

            _logic = diMap.Resolve<IInternalMarketingPreferenceLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfullyCreateMarketingPreferences() {
            MarketingPreferenceModel model = new MarketingPreferenceModel();

            model.Email = "test@test.com";
            model.BranchId = "FDF";
            model.CurrentCustomer = true;
            model.LearnMore = true;
            model.RegisteredOn = DateTime.Now;

            _logic.CreateMarketingPreference(model);
        }
        #endregion
    }
}
