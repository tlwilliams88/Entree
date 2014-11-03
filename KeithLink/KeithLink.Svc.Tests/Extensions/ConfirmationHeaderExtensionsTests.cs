﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using System.IO;

namespace KeithLink.Svc.Test.Extensions
{
    [TestClass]
    public class ConfirmationHeaderExtensionsTests
    {
        private const string TEST_FILE = "Assets\\confirmation_file.txt";

        [TestMethod]
        public void ShouldParseValidConfirmationHeaderLine()
        {
            StreamReader testFile = new StreamReader(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

            ConfirmationHeader testHeader = new ConfirmationHeader();

            testHeader.Parse(testFile.ReadLine());

            Assert.IsTrue(testHeader.CustomerNumber == "010166");
        }
    }
}
