using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Impl.Repository.Logging;

using BEKlibrary.EventLog.Datalayer;

using Xunit;
using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEKlibrary;
using BEKlibrary.EventLog.BusinessLayer;

using FluentAssertions;

namespace KeithLink.Common.Impl.Tests.Unit.Repository.Logging
{
    public class EventLogQueueRepositoryImplTest
    {
        #region Setup
        public class MockDependents {
            public Mock<IQueueRepository> QueueRepository { get; set; }
            public Mock<EventLogQueueRepositoryImpl> EventLogQueueRepository { get; set; }

            public static Mock<IQueueRepository> MakeMockQueueRepository() {
                var mock = new Mock<IQueueRepository>();

                mock.Setup(m => m.PublishLogMessage(It.IsAny<LogMessage>()));

                return mock;
            }
        }

        private static MockDependents MakeMockDependents() {
            var mocks = new MockDependents();
            mocks.QueueRepository = MockDependents.MakeMockQueueRepository();

            return mocks;
        }

        #endregion


        #region WriteInformationTests
        public class WriteInformationTests {
            [Fact]
            public void GoodInformationLog_CallsPublishToQueue() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                    .Verify(x => x.PublishLogMessage(It.IsAny<LogMessage>()), Times.Once, "Not called with expected LogMessage");
            }
        }
        #endregion
    }
}
