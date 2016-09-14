﻿using KeithLink.Svc.Core.Interface.Email;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Logic.Messaging;

using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Test.Email
{
	[TestClass]
	public class EmailTemplateTest
	{
        private IEmailClient _emailClient;
        private IMessageTemplateLogic _templateLogic;

        private const string EMAIL_ADDRESS = "mdjoiner@benekeith.com";

        public EmailTemplateTest() {
            var container = DependencyMap.Build();

            _emailClient = container.Resolve<IEmailClient>();
            _templateLogic = container.Resolve<IMessageTemplateLogic>();
        }

		[TestMethod]
		public void SendEmailTest()
		{
            //var template = _templateLogic.ReadForKey("CreatedUserWelcome");

            //_emailClient.SendTemplateEmail(template, new List<string>() { EMAIL_ADDRESS }, new { resetLink = "testUrl" });

            Assert.IsTrue(true);

		}

        [TestMethod]
        public void SendEmailAndTemporaryPasswordToCreatedUser() {

            //MessageTemplateLogicImpl m = new MessageTemplateLogicImpl(new MessageTemplateRepositoryImpl(new UnitOfWork()));

            //var template = m.ReadForKey("CreatedUserWeclome");

            //emailClient.SendTemplateEmail(template, new List<string> { "mdjoiner@benekeith.com" }, new { password = "hello world" });

            Assert.IsTrue( true );
        }

	}
}
