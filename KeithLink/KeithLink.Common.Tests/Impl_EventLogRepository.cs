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
            KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Impl.Logging.EventLogRepositoryImpl("KeithLink.Common.Tests");

            log.WriteInformationLog("Testing");

            Assert.IsTrue(true);
        }
    }
}
