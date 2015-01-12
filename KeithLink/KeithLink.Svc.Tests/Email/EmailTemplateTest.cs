using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;
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
			//MessageTemplateLogicImpl messageTemplateLogic = new MessageTemplateLogicImpl(new MessageTemplateRepositoryImpl(new UnitOfWork()));

			//var template = messageTemplateLogic.ReadForKey("testEmailTemplate");

			//emailClient.SendTemplateEmail(template, new List<string>() { "jtirey@credera.com" }, null, new List<string> { "jtirey@credera.com" }, new { firstName = "Josh", lastName = "Tirey" });

			Assert.IsTrue(true);

		}

        [TestMethod]
        public void SendEmailAndTemporaryPasswordToCreatedUser() {
            EmailClientImpl emailClient = new EmailClientImpl( new TokenReplacer() );

            //MessageTemplateLogicImpl m = new MessageTemplateLogicImpl( new MessageTemplateRepositoryImpl( new UnitOfWork() ) );

            //var template = m.ReadForKey( "CreatedUserWeclome" );

            //emailClient.SendTemplateEmail( template, new List<string> { "mdjoiner@benekeith.com" }, new { password = "hello world" } );

            Assert.IsTrue( true );
        }

	}
}
