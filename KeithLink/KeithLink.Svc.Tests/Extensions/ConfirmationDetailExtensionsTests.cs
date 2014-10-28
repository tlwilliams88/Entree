using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using System.IO;

namespace KeithLink.Svc.Test.Extensions
{
    [TestClass]
    public class ConfirmationDetailExtensionsTests
    {
        private const string TEST_FILE = "Assets\\confirmation_file.txt";

        [TestMethod]
        public void DetailLineShouldParseSuccessfully()
        {
            StreamReader testFile = new StreamReader(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

            ConfirmationDetail testDetail = new ConfirmationDetail();

            testFile.ReadLine(); // Skip header record

            testDetail.Parse(testFile.ReadLine());

            Assert.IsTrue(testDetail.RecordNumber == "00001");
        }
    }
}
