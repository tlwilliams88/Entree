using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.Invoices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Repositories.Invoices {
    [TestClass]
    public class ImagingRepoTests {
        #region attributes
        private EventLogRepositoryImpl _log;
        private ImagingRepositoryImpl _imgRepo;
        #endregion

        #region ctor
        public ImagingRepoTests() {
            _log = new EventLogRepositoryImpl("Entree Unit Tests");
            _imgRepo = new ImagingRepositoryImpl(_log);
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfullyConnect() {
            string token = _imgRepo.Connect();

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void SuccessfullyFindDocumentId() {
            string token = _imgRepo.Connect();

            List<string> docIds = _imgRepo.GetDocumentIds(token, new Core.Models.SiteCatalog.UserSelectedContext() { BranchId = "FDF", CustomerId = "024418" }, "15798232");

            Assert.IsNotNull(docIds);
        }

        [TestMethod]
        public void SuccessfullyGetImagePreviews() {
            string token = _imgRepo.Connect();

            List<string> docIds = _imgRepo.GetDocumentIds(token, new Core.Models.SiteCatalog.UserSelectedContext() { BranchId = "FDF", CustomerId = "024418" }, "15798232");

            List<string> images = new List<string>();

            foreach (string docId in docIds) {
               images.AddRange(_imgRepo.GetImages(token, docId));
            }

            Assert.IsTrue(images.Count > 0);
        }
        #endregion
    }
}
