using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using Autofac;
using IContainer = Autofac.IContainer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class DsrRepositoryTests
    {
        #region Attributes

        private IDsrRepository _repo;
        private IUnitOfWork _uow;

        #endregion

        #region ctor

        public DsrRepositoryTests()
        {
            IContainer depMap = DependencyMap.Build();
            _repo = depMap.Resolve<IDsrRepository>();
            _uow = depMap.Resolve<IUnitOfWork>();
        }

        #endregion

        #region Methods
        
        [TestMethod]
        public void GetAllDsrInfo()
        {
            List<KeithLink.Svc.Core.Models.EF.Dsr> dsrList = _repo.ReadAll().ToList();

            if (dsrList.Count == 0)
            {
                Assert.Fail("No DSRs found");
            }
            else
            {
                Assert.IsTrue(true);
            }

        }

        #endregion
    }
}
