using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Models.PowerMenu;
using System.IO;

namespace KeithLink.Svc.Test.PowerMenu {
    [TestClass]
    public class PowerMenuSystemRequestModelTest {

        [TestMethod]
        public void PowerMenuSerializingTest() {
            PowerMenuSystemRequestModel pm = new PowerMenuSystemRequestModel();

            pm.Login.AdminUsername = "MATTTEST";
            pm.Login.AdminPassword = "MATTTEST";

            pm.User.Username = "mj@thismat.com";
            pm.User.Password = "RANDOM";
            pm.User.EmailAddress = "mj@thismat.com";
            pm.User.ContactName = "Matt";
            pm.User.CustomerNumber = "123456";
            pm.User.PhoneNumber = "817-507-5415";
            pm.User.State = "TX";
            pm.Operation = PowerMenuSystemRequestModel.Operations.Edit;


            XmlSerializer s = new XmlSerializer( typeof( PowerMenuSystemRequestModel ) );

            //TextWriter w = new StreamWriter(@"C:\test\powermenu\testxml.xml");
            TextWriter w = new StringWriter();

            try {
                s.Serialize( w, pm );
            } catch (Exception ex) {
                Assert.Fail( ex.Message );
            }
           
        }
    }
}
