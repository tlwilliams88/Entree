using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autofac;

//using FluentAssertions;
using FluentAssertions.Json;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Impl.Logic;

using Moq;
using Newtonsoft.Json.Linq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic
{
    public class FlipSnackApiClientTests : BaseDITests
    {
        private static FlipSnackApiClient InstantiateTestSubject()
        {
            IEventLogRepository eventLog = new Mock<IEventLogRepository>().Object;

            ContainerBuilder containerBuilder = GetTestsContainer();
            containerBuilder.RegisterInstance(eventLog)
                .As<IEventLogRepository>();

            IContainer container = containerBuilder.Build();

            return container.Resolve<FlipSnackApiClient>();
        }

        [Fact]
        public void GetList_InvalidCredentials()
        {
            // arrange
            FlipSnackApiClient testSubject = InstantiateTestSubject();

            // expect
            int expected = 41;  // Invalid credentials

            // act
            JObject result = testSubject.GetList();

            // assert
            result["code"]
                .Should()
                .BeEquivalentTo(expected);
        }

        [Fact]
        public void GetEmbed_InvalidCredentials()
        {
            // arrange
            FlipSnackApiClient testSubject = InstantiateTestSubject();

            // expect
            int expected = 41;  // Invalid credentials

            // act
            JObject result = testSubject.GetEmbed();

            // assert
            result["code"]
                .Should()
                .BeEquivalentTo(expected);
        }

    }
}
