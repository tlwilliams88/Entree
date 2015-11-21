using KeithLink.Svc.Core.Interface.Profile;

using Autofac;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.Dsr
{
    [TestClass]
    public class DsrLogicTests
    {
        #region Attributes
        private IDsrLogic _logic;
        #endregion

        #region ctor
        public DsrLogicTests()
        {
            var container = DependencyMap.Build();
            _logic = container.Resolve<IDsrLogic>();
        }
        #endregion

        #region Methods

        [TestMethod]
        public void GetAllDsrInfo()
        {
            List<Core.Models.Profile.Dsr> dsrList = _logic.GetAllDsrInfo();

            Assert.IsTrue(dsrList.Count > 0);
        }

        #endregion
    }
}
