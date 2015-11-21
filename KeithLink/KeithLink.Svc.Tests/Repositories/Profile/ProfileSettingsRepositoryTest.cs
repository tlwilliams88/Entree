using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Autofac;
using Autofac.Core;
using IContainer = Autofac.IContainer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace KeithLink.Svc.Test.Repositories.Profile
{
    [TestClass]
    public class ProfileSettingsReporsitoryTest
    {
        #region Attributes

        private ISettingsRepository _repo;
        private IUnitOfWork _uow;

        #endregion

        #region ctor

        public ProfileSettingsReporsitoryTest()
        {
            IContainer depMapo = DependencyMap.Build();
            _repo = depMapo.Resolve<ISettingsRepository>();
            _uow = depMapo.Resolve<IUnitOfWork>();
        }

        #endregion  

        #region tests



        #endregion

    }
}
