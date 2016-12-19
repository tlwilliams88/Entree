using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Impl.Repository.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using KeithLink.Svc.Core.Models.EF;

using Autofac;

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Lists {
    /// <summary>
    /// Summary description for CustomInventoryItemsRepositoryTests
    /// </summary>
    [TestClass]
    public class CustomInventoryItemsRepositoryTests {

        #region attributes 
        private IUnitOfWork _uow;
        private ICustomInventoryItemsRepository _repo;

        private TestContext testContextInstance;

        private List<CustomInventoryItem> _data;
        #endregion

        public CustomInventoryItemsRepositoryTests() {
            IContainer container = DependencyMap.Build();

            _repo = container.Resolve<ICustomInventoryItemsRepository>();
            _uow = container.Resolve<IUnitOfWork>();

            GenerateSampleData();
        }

        public void GenerateSampleData() {
            
        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
         public void MyTestInitialize() {
            _data = new List<CustomInventoryItem>();

            _data.Add(new CustomInventoryItem() {
                ItemNumber = "123456",
                CustomerNumber = "000001",
                BranchId = "FAM",
                Name = "My custom item #1",
                Brand = "My custom brand #1",
                Pack = "1",
                Size = "1",
                Vendor = "00011",
                Each = false,
                CasePrice = (decimal)50.99,
                PackagePrice = (decimal)0
            });

            _data.Add(new CustomInventoryItem() {
                ItemNumber = "654321",
                CustomerNumber = "000001",
                BranchId = "FAM",
                Name = "My custom item #2",
                Brand = "My custom brand #2",
                Pack = "5",
                Size = "5",
                Vendor = "00012",
                Each = true,
                CasePrice = (decimal)20.00,
                PackagePrice = (decimal)5.00
            });

            _data.Add(new CustomInventoryItem() {
                ItemNumber = "654321",
                CustomerNumber = "000001",
                BranchId = "FAM",
                Name = "My custom item #3",
                Brand = "My custom brand #3",
                Pack = "5",
                Size = "5",
                Vendor = "00012",
                Each = true,
                CasePrice = (decimal)20.00,
                PackagePrice = (decimal)5.00
            });

            _data.Add(new CustomInventoryItem() {
                Id = 5,
                ItemNumber = "654321",
                CustomerNumber = "000002",
                BranchId = "FDF",
                Name = "My custom item #3",
                Brand = "My custom brand #3",
                Pack = "5",
                Size = "5",
                Vendor = "00013",
                Each = true,
                CasePrice = (decimal)20.00,
                PackagePrice = (decimal)5.00
            });
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[TestMethod]
        //public void CanAddNewCustomInventoryItem() {
        //    // Save first test item in list
        //    _uow.Context.CustomInventoryItems.Add(_data[0]);
        //    _uow.SaveChangesAndClearContext();

        //    CustomInventoryItems item = _repo.Get(_data[0].BranchId, _data[0].CustomerNumber, _data[0].ItemNumber);

        //    Assert.IsNotNull(item);
        //}

        //[TestMethod]
        //public void CanAddMultipleItems() {
        //    _uow.Context.CustomInventoryItems.AddRange(_data);
        //    _uow.SaveChangesAndClearContext();

        //    List<CustomInventoryItems> items = _repo.Get(_data[2].BranchId, _data[2].CustomerNumber);

        //    Assert.AreEqual(items.Count, 2);
        //}
    }
}
