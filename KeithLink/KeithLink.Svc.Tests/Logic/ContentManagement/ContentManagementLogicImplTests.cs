using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement;

using Autofac;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac.Extras.Moq;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Test.Repositories.Mock;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Test.Logic.ContentManagement {
    [TestClass]
    public class ContentManagementLogicImplTests {
        #region attributes
        private readonly IContentManagementLogic _logic;
        #endregion

        #region ctor
        public ContentManagementLogicImplTests() {
            var container = DependencyMap.Build();

            _logic = container.Resolve<IContentManagementLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetContentSuccessfully() {
            List<ContentItemViewModel> items = _logic.ReadContentForBranch("FDF");
        }
        [TestMethod]
        public void ContentManagementLogicImplTests_LogHit()
        {
            // Arrange
            var mockAuditLog = new AuditLogRepositoryMock();
            var mockUser = new UserProfile()
            {
                FirstName = "Test",
                LastName = "User",
                UserName = "testUserName"
            };
            var mockSelected = new UserSelectedContext()
            {
                BranchId= "tstBranch",
                CustomerId = "tstCustomerId"
            };
            var mockClicked = new ContentItemClickedModel()
            {
                CampaignId = "tstCampainId",
                TagLine = "tstTagLine"
            };
            var mockExpected = new AuditLogEntry()
            {
                EntryType =Common.Core.Enumerations.AuditType.MarketingCampaignClicked,
                Actor = mockUser.UserName,
                Information = string.Format("{0}|{1},{2}", mockSelected.CustomerId, mockClicked.CampaignId, mockClicked.TagLine)
            };
            var CMSLogic = new Impl.Logic.ContentManagement.ContentManagementLogicImpl(null, null, mockAuditLog);

            // Action
            CMSLogic.LogHit(mockUser, mockSelected, mockClicked);

            // Assert
            Assert.AreEqual<int>((int)mockExpected.EntryType, (int)mockAuditLog.Entries[0].EntryType);
            Assert.AreEqual<string>(mockExpected.Actor, mockAuditLog.Entries[0].Actor);
            Assert.AreEqual<string>(mockExpected.Information, mockAuditLog.Entries[0].Information);
        }
        #endregion
    }
}
