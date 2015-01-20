using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class ConfirmationLogicImplTests
    {
        private const string TEST_FILE = "Assets\\confirmation_file.txt";

        [TestMethod]
        public void ConfirmationFileShouldParseAndSendToRabbitMQ()
        {
            UnitOfWork uow = new UnitOfWork();
            OrderConversionLogicImpl conversionLogic = new OrderConversionLogicImpl(new OrderHistoyrHeaderRepositoryImpl(uow), uow, new EventLogRepositoryImpl("Unit Tests")); 

            ConfirmationLogicImpl logic = new ConfirmationLogicImpl(new EventLogRepositoryImpl("Entree Tests"),
                                                                    new SocketListenerRepositoryImpl(),
                                                                    new KeithLink.Svc.Impl.Repository.Queue.GenericQueueRepositoryImpl(),
                                                                    conversionLogic, 
                                                                    uow);

            StreamReader testFile = new StreamReader(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

            List<string> data = new List<string> { };

            while (testFile.EndOfStream == false)
            {
                data.Add(testFile.ReadLine());
            }

            testFile.Close();
            testFile.Dispose();

            logic.ProcessFileData(data.ToArray());
        }
    }
}
