﻿using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders.Confirmations;
using KeithLink.Common.Impl.Logging;
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
            ConfirmationLogicImpl logic = new ConfirmationLogicImpl(new EventLogRepositoryImpl("Entree Tests"),
                                                                    new SocketListenerRepositoryImpl(),
                                                                    new ConfirmationQueueRepositoryImpl(),
                                                                    new KeithLink.Svc.Impl.Repository.Queue.GenericQueueRepositoryImpl());

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
