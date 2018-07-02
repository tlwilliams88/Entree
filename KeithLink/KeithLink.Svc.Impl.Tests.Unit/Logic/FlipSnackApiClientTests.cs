using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Impl.Logic;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic
{
    public class FlipSnackApiClientTests : BaseDITests
    {
        private static FlipSnackApiClient InstantiateTestSubject()
        {
            Mock<IEventLogRepository> eventLog = new Mock<IEventLogRepository>();

            ContainerBuilder containerBuilder = GetTestsContainer();
            containerBuilder.RegisterInstance(eventLog)
                .As<IEventLogRepository>();

            IContainer container = containerBuilder.Build();

            return container.Resolve<FlipSnackApiClient>();
        }

        [Fact]
        public void GoodData_GetEmbed()
        {
            // arrange
            FlipSnackApiClient testSubject = InstantiateTestSubject();

            // act
            string results = testSubject.GetEmbed();


            // expect
            string expected = "";

            // assert
            results.Count()
                .Should()
                .BeGreaterThan(0);

            //results[0]
            //    .CampaignId
            //    .Should()
            //    .Be(expected);
        }

    }
}
