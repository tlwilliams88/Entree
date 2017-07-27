using System;
using System.Collections.Generic;

using Autofac;
using FluentAssertions;

using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.List;

using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Extensions;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Extensions
{
    public class ListModelExtensionsTests
    {
        [Fact]
        public void GoodCustomerIdAndBranch_CallsDeleteDetail()
        {
            // arrange
            ListModel list = new ListModel() {
                                                 Type = ListType.Mandatory,
                                                 ReadOnly = true
                                             };
            UserProfile user = new UserProfile() {
                                                     RoleName = Constants.ROLE_NAME_SYSADMIN
            };
            var expected = false;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user).ReadOnly
                                                .Should()
                                                .Be(expected);

        }
    }
}
