using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine;

using Autofac;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.ContentManagement {
    [TestClass]
    public class ContentManagementExternalRepositoryImplTests {
        #region attributes
        private readonly IContentManagementExternalRepository _contentRepo;
        #endregion

        #region ctor
        public ContentManagementExternalRepositoryImplTests(){
            var container = DependencyMap.Build();

            _contentRepo = container.Resolve<IContentManagementExternalRepository>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void ReadAllContentSuccessfully() {
            List<ContentItem> items = _contentRepo.GetAllContent();
        }
        #endregion
    }
}
