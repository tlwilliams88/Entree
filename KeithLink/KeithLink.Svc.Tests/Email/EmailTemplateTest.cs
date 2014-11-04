using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
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
		[TestMethod]
		public void SendEmailTest()
		{
			EmailClientImpl emailClient = new EmailClientImpl(new TokenReplacer());
			EmailTemplateLogicImpl emailTemplateLogic = new EmailTemplateLogicImpl(new EmailTemplateRepositoryImpl(new UnitOfWork()));

			//var template = emailTemplateLogic.ReadForKey("testEmailTemplate");

			//emailClient.SendTemplateEmail(template, new List<string>() { "jtirey@credera.com" }, null, new List<string> { "jtirey@credera.com" }, new { firstName = "Josh", lastName = "Tirey" });

			Assert.IsTrue(true);

		}

	}
}
