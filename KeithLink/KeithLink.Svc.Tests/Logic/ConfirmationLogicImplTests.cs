using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class ConfirmationLogicImplTests {
        #region attributes
        private const string TEST_FILE = "Assets\\confirmation_file.txt";
        private readonly IConfirmationLogic _logic;
        #endregion

        #region ctor
        public ConfirmationLogicImplTests() {
            IContainer diContainer = DependencyMap.Build();

            _logic = diContainer.Resolve<IConfirmationLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void ConfirmationFileShouldParseAndSendToRabbitMQ()
        {
			StreamReader testFile = new StreamReader(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

            List<string> data = new List<string> { };

            while (testFile.EndOfStream == false)
            {
                data.Add(testFile.ReadLine());
            }

            testFile.Close();
            testFile.Dispose();

            _logic.ProcessFileData(data.ToArray());
        }
        #endregion
    }
}
