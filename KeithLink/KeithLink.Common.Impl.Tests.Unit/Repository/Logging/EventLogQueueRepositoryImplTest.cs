using System;
using System.Configuration;

using BEKlibrary;
using BEKlibrary.EventLog.BusinessLayer;

using KeithLink.Common.Impl.Repository.Logging;

using Moq;

using Xunit;

namespace KeithLink.Common.Impl.Tests.Unit.Repository.Logging {
    public class EventLogQueueRepositoryImplTest {
        #region WriteInformationTests
        public class WriteInformationTests {
            [Fact]
            public void GoodInformationLog_CallsPublishToQueueAndSetsMachineName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();

                // act
                log.WriteInformationLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Length > 0)),
                                  Times.Once(),
                                  "Machine name did not get set");
            }

            [Fact]
            public void GoodInformationLog_CallsPublishToQueueAndSetsStackTrace() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();

                // act
                log.WriteInformationLog("Ooops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Exception.StackTrace.Length > 0)),
                                  Times.Once,
                                  "StackTrace is not being set");
            }

            [Fact]
            public void GoodInformationLog_CallsPublishToQueueWithExceptionMessage() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = "Something went wrong!";

                // act
                log.WriteInformationLog(expected, MakeException());

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message did not match");
            }

            [Fact]
            public void GoodInformationLog_HasEnvironmentSet() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();
                string expectedConfiguration = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteInformationLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expectedConfiguration))),
                                  Times.Once(),
                                  "Environment not set properly");
            }

            [Fact]
            public void GoodInformationLog_HasTheProperSeverity() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();
                int expectedSeverity = 1;

                // act
                log.WriteInformationLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expectedSeverity))),
                                  Times.Once(),
                                  "Severity level not set to information");
            }

            [Fact]
            public void GoodInformationLogNoException_CallsPublishToQueue() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.IsAny<LogMessage>()), Times.Once, "Not called with expected LogMessage");
            }

            [Fact]
            public void GoodInformationLogNoException_HasApplicationName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = ConfigurationManager.AppSettings["AppName"];

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Name.Equals(expected))),
                                  Times.Once,
                                  "Application Name not set properly");
            }

            [Fact]
            public void GoodInformationLogNoException_HasMachineName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = Environment.MachineName;

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Equals(expected))),
                                  Times.Once,
                                  "Machine.Name not set properly");
            }

            [Fact]
            public void GoodInformationLogNoException_HasMessage() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = "test";

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message was not set properly");
            }

            [Fact]
            public void GoodInformationLogNoException_HasProperEnvironment() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expected))),
                                  Times.Once,
                                  "Application Environment not set properly");
            }

            [Fact]
            public void GoodInformationLogNoException_HasProperSeverity() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                int expected = 1;

                // act
                log.WriteInformationLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expected))),
                                  Times.Once,
                                  "Severity level not set properly");
            }
        }
        #endregion

        #region WriteWarningTests
        public class WriteWarningTests {
            [Fact]
            public void GoodWarningLog_CallsPublishToQueueAndSetsMachineName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();

                // act
                log.WriteWarningLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Length > 0)),
                                  Times.Once(),
                                  "Machine name did not get set");
            }

            [Fact]
            public void GoodWarningLog_CallsPublishToQueueAndSetsStackTrace() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();

                // act
                log.WriteWarningLog("Ooops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Exception.StackTrace.Length > 0)),
                                  Times.Once,
                                  "StackTrace is not being set");
            }

            [Fact]
            public void GoodWarningLog_CallsPublishToQueueWithExceptionMessage() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = "Something went wrong!";

                // act
                log.WriteWarningLog(expected, MakeException());

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message did not match");
            }

            [Fact]
            public void GoodWarningLog_HasEnvironmentSet() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();
                string expectedConfiguration = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteWarningLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expectedConfiguration))),
                                  Times.Once(),
                                  "Environment not set properly");
            }

            [Fact]
            public void GoodWarningLog_HasTheProperSeverity() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                Exception exception = MakeException();
                int expectedSeverity = 2;

                // act
                log.WriteWarningLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expectedSeverity))),
                                  Times.Once(),
                                  "Severity level not set to information");
            }

            [Fact]
            public void GoodWarningLogNoException_CallsPublishToQueue() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.IsAny<LogMessage>()), Times.Once, "Not called with expected LogMessage");
            }

            [Fact]
            public void GoodWarningLogNoException_HasApplicationName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = ConfigurationManager.AppSettings["AppName"];

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Name.Equals(expected))),
                                  Times.Once,
                                  "Application Name not set properly");
            }

            [Fact]
            public void GoodWarningLogNoException_HasMachineName() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = Environment.MachineName;

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Equals(expected))),
                                  Times.Once,
                                  "Machine.Name not set properly");
            }

            [Fact]
            public void GoodWarningLogNoException_HasMessage() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = "test";

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message was not set properly");
            }

            [Fact]
            public void GoodWarningLogNoException_HasProperEnvironment() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expected))),
                                  Times.Once,
                                  "Application Environment not set properly");
            }

            [Fact]
            public void GoodWarningLogNoException_HasProperSeverity() {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                int expected = 2;

                // act
                log.WriteWarningLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expected))),
                                  Times.Once,
                                  "Severity level not set properly");
            }
        }
        #endregion

        #region WriteErrorTests
        public class WriteErrorTests
        {
            [Fact]
            public void GoodErrorLogNoException_CallsPublishToQueue()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.IsAny<LogMessage>()), Times.Once, "Not called with expected LogMessage");
            }

            [Fact]
            public void GoodErrorLogNoException_HasMessage()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var expected = "test";

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message was not set properly");
            }

            [Fact]
            public void GoodErrorLogNoException_HasProperSeverity()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var expected = 3;

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expected))),
                                  Times.Once,
                                  "Severity level not set properly");
            }

            [Fact]
            public void GoodErrorLogNoException_HasMachineName()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var expected = Environment.MachineName;

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Equals(expected))),
                                  Times.Once,
                                  "Machine.Name not set properly");
            }

            [Fact]
            public void GoodErrorLogNoException_HasApplicationName()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var expected = ConfigurationManager.AppSettings["AppName"];

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Name.Equals(expected))),
                                  Times.Once,
                                  "Application Name not set properly");
            }

            [Fact]
            public void GoodErrorLogNoException_HasProperEnvironment()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var expected = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteErrorLog("test");

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expected))),
                                  Times.Once,
                                  "Application Environment not set properly");
            }


            [Fact]
            public void GoodErrorLog_CallsPublishToQueueWithExceptionMessage()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                string expected = "Something went wrong!";

                // act
                log.WriteErrorLog(expected, MakeException());

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Message.Equals(expected))),
                                  Times.Once,
                                  "Message did not match");
            }

            [Fact]
            public void GoodErrorLog_CallsPublishToQueueAndSetsStackTrace()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var exception = MakeException();

                // act
                log.WriteErrorLog("Ooops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Exception.StackTrace.Length > 0)),
                                  Times.Once,
                                  "StackTrace is not being set");
            }

            [Fact]
            public void GoodErrorLog_CallsPublishToQueueAndSetsMachineName()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var exception = MakeException();

                // act
                log.WriteErrorLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Machine.Name.Length > 0)),
                                  Times.Once(),
                                  "Machine name did not get set");
            }

            [Fact]
            public void GoodErrorLog_HasTheProperSeverity()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var exception = MakeException();
                var expectedSeverity = 3;

                // act
                log.WriteErrorLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.EntryType.Id.Equals(expectedSeverity))),
                                  Times.Once(),
                                  "Severity level not set to information");
            }


            [Fact]
            public void GoodErrorLog_HasEnvironmentSet()
            {
                // arrange
                MockDependents dependents = MakeMockDependents();
                EventLogQueueRepositoryImpl log = new EventLogQueueRepositoryImpl(dependents.QueueRepository.Object);
                var exception = MakeException();
                var expectedConfiguration = ConfigurationHelper.GetActiveConfiguration();

                // act
                log.WriteErrorLog("Oops!", exception);

                // assert
                dependents.QueueRepository
                          .Verify(x => x.PublishLogMessage(It.Is<LogMessage>(m => m.Application.Environment.Equals(expectedConfiguration))),
                                  Times.Once(),
                                  "Environment not set properly");
            }
        }
        #endregion

        #region Setup
        public class MockDependents {
            public Mock<IQueueRepository> QueueRepository { get; set; }

            public Mock<EventLogQueueRepositoryImpl> EventLogQueueRepository { get; set; }

            public static Mock<IQueueRepository> MakeMockQueueRepository() {
                Mock<IQueueRepository> mock = new Mock<IQueueRepository>();

                mock.Setup(m => m.PublishLogMessage(It.IsAny<LogMessage>()));

                return mock;
            }
        }

        private static MockDependents MakeMockDependents() {
            MockDependents mocks = new MockDependents();
            mocks.QueueRepository = MockDependents.MakeMockQueueRepository();

            return mocks;
        }

        private static Exception MakeException() {
            try {
                throw new Exception("Something went wrong inner!");
            } catch (Exception ex) {
                return new Exception("", ex);
            }
        }
        #endregion
    }
}