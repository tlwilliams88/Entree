using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement;

using Autofac;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.ContentManagement {
    [TestClass]
    public class ContentManagementLogicImplTests {
        #region attributes
        private readonly IContentManagementLogic _logic;
        #endregion

        #region ctor
        public ContentManagementLogicImplTests() {
            var container = DependencyMap.Build();

            _logic = container.Resolve<IContentManagementLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetContentSuccessfully() {
            List<ContentItemViewModel> items = _logic.ReadContentForBranch("FDF");
        }
        #endregion
    }
}
