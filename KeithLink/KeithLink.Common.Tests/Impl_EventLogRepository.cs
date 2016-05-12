using KeithLink.Common.Impl.Repository.Logging;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Common.Tests
{
    [TestClass]
    public class Impl_EventLogRepository
    {
        [TestMethod]
        public void WriteTestInformationLog()
        {
            EventLogRepositoryImpl log = new EventLogRepositoryImpl("KeithLink.Common.Tests");

            log.WriteInformationLog("Testing");

            Assert.IsTrue(true);
        }
    }
}
