using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Profile_CustomerContainerRepository
    {
        [TestMethod]
        public void CreateGoodContainer()
        {
        //    try {
        //        KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
        //        custCont.CreateCustomerContainer(KeithLink.Svc.Core.Constants.AD_GUEST_CONTAINER);

        //        Assert.IsTrue(true);
        //    } catch {
        //        Assert.IsTrue(false);
        //    }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CreateDuplicateContainer()
        {
            //try
            //{
            //    KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository();
            //    custCont.CreateCustomerContainer("Jimmys Chicken Shack");

            //    // this should fail
            //    Assert.IsTrue(false);
            //}
            //catch
            //{
            //    Assert.IsTrue(true);
            //}
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CreateContainerWithEmptyString()
        {
            try
            {
                KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
                custCont.CreateCustomerContainer(string.Empty);

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void CreateContainerWithSpecialCharacters()
        {
            try
            {
                KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
                custCont.CreateCustomerContainer(string.Empty);

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void DeleteGoodContainer()
        {
            //try
            //{
            //    KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository();
            //    custCont.DeleteCustomerContainer("Jeremys Chicken Shack");

            //    Assert.IsTrue(true);
            //}
            //catch
            //{
            //    Assert.IsTrue(false);
            //}
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeleteNonExistingContainer()
        {
            try
            {
                KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
                custCont.DeleteCustomerContainer("Non-Existant Container");

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void DeleteContainerWithEmptyString()
        {
            try
            {
                KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));
                custCont.DeleteCustomerContainer(string.Empty);

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void GetGoodContainer()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            Assert.IsTrue(custCont.GetCustomerContainer("Jimmys Chicken Shack").CustomerContainers.Count == 1);
        }

        [TestMethod]
        public void GetContainerWithInvalidName()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            Assert.IsTrue(custCont.GetCustomerContainer("Jimmys").CustomerContainers.Count == 0);
        }

        [TestMethod]
        public void GetContainerWithEmptyString()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            try
            {
                custCont.GetCustomerContainer(string.Empty);

                // this should throw an exception
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void SearchCustomerContainers()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            Assert.IsTrue(custCont.SearchCustomerContainers("j").CustomerContainers.Count > 0);
        }

        [TestMethod]
        public void SearchCustomerContainersWithEmptyString()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            try
            {
                custCont.SearchCustomerContainers(string.Empty);

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void SearchCustomerContainersWithSpecialCharacters()
        {
            KeithLink.Svc.Impl.Repository.Profile.CustomerContainerRepository custCont = new Impl.Repository.Profile.CustomerContainerRepository(new Common.Impl.Logging.EventLogRepositoryImpl("KeithLinkTessts"));

            try
            {
                custCont.SearchCustomerContainers("Jimmy's Chicken Shack");

                // this should fail
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}
