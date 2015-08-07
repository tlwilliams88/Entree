using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Autofac;
using Autofac.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        #region Attributes

        private ICustomerRepository _repo;
        private IUnitOfWork _uow;
        

        #endregion

        #region ctor

        public CustomerRepositoryTests()
        {
            IContainer depMap = DependencyMap.Build();
            _repo = depMap.Resolve<ICustomerRepository>();
            _uow = depMap.Resolve<IUnitOfWork>();
            
        }

        #endregion

        #region Methods

        [TestMethod]
        public void GetCustomersByNameOrNumber()
        {
            List<Customer> customerList = _repo.GetCustomersByNameOrNumber("Swaringen");

            Assert.IsTrue(customerList.Count > 0);
        }

        #endregion
    }
}
