using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic.InternalSrv
{
    [TestClass]
    public class InternalSvc_InternalListLogic_ReadPagedList_Tests
    {
        private long ListId = 1;
        private IListLogic internalListLogic;
        private UserSelectedContext userSelectedContext;
        private UserProfile userProfile;
        private PagingModel pagingModel;
        [TestInitialize]
        public void TestInitialize()
        {
            var _cbuilder = DependencyMap.Init();
            //*******************************************
            //Mock Items
            //*******************************************
            _cbuilder.RegisterType<ReadPagedList_UnitOfWorkMock>().As<IUnitOfWork>().InstancePerLifetimeScope();

            _cbuilder.RegisterStandard();
            var _container = _cbuilder.Build();

            internalListLogic = _container.Resolve<IListLogic>();

            userSelectedContext = TestSessionObject.TestUserContext;
            userProfile = TestSessionObject.TestAuthenticatedUser;
            pagingModel = new PagingModel();
        }
        [TestMethod]
        public void InitialRun()
        {
            PagedListModel list = internalListLogic.ReadPagedList(userProfile, userSelectedContext, ListId, pagingModel);
            Assert.IsNotNull(list);
            Assert.IsNotNull(list.Items);
        }
        [TestMethod]
        public void ValidItemInList()
        {
            PagedListModel list = internalListLogic.ReadPagedList(userProfile, userSelectedContext, ListId, pagingModel);
            Assert.IsTrue(list.Items.Results[0].IsValid); // Item for Apples; valid
        }
        [TestMethod]
        public void InValidItemInList()
        {
            PagedListModel list = internalListLogic.ReadPagedList(userProfile, userSelectedContext, ListId, pagingModel);
            Assert.IsFalse(list.Items.Results[1].IsValid); // ItemNumber 999999; invalid
        }
    }
}
