using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;
using KeithLink.Svc.Impl.Logic.ETL;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Test.Logic.ETL
{
    [TestClass]
    public class ItemImportLogicImplTests
    {
        #region Attributes
        private IContainer _container;
        private IItemImport _itemImportLogic;
        #endregion

        public ItemImportLogicImplTests()
        {
            _container = DependencyMapFactory.GetInternalServiceContainer().Build();
            _itemImportLogic = _container.Resolve<IItemImport>();
        }

        /// <summary>
        /// Unit test used to kick of the ImportItems process
        /// </summary>
        [TestMethod]
        public void ItemImportLogic_ImportItems()
        {
            // we don't want to start a load of items when the tests are evaluated normally
            //_itemImportLogic.ImportItems();
        }

        /// <summary>
        /// Unit test used to kick of the ImportUnfiItems process
        /// </summary>
        [TestMethod]
        public void ItemImportLogic_ImportUNFIItems()
        {
            // we don't want to start a load of items when the tests are evaluated normally
            //_itemImportLogic.ImportUNFIItems();
        }
    }
}
