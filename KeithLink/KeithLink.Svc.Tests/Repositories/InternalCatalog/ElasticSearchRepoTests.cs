using KeithLink.Svc.Core.Interface.InternalCatalog;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Repositories.InternalCatalog {
    [TestClass]
    public class ElasticSearchRepoTests {
        #region attributes
        private IElasticSearchRepository _repo;
        #endregion

        #region ctor
        public ElasticSearchRepoTests() {
            IContainer diMap = DependencyMap.Build();

            _repo = diMap.Resolve<IElasticSearchRepository>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfullyCreateEmptyIndex() {
            _repo.CreateEmptyIndex("FDF");
        }
        #endregion
    }
}
