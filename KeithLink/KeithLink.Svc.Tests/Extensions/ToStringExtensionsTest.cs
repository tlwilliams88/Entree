// KeithLink
using KeithLink.Common.Core.Extensions;

// Core
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Extensions {
    [TestClass]
    public class ToStringExtensionTests {

        [TestMethod]
        public void ToYearFirstDateWithTimeShouldParseDateTimeStrings() {
            string testValue = "08/24/2020 01:02:03";

            Assert.Equals( "20200824010203", testValue.ToYearFirstDateWithTime() );
        }

        public void ToYearFirstDateShouldParseDateStrings() {
            string testValue = "08/24/2020";

            Assert.Equals( "20200824", testValue.ToYearFirstDate() );
        }

    }
}
