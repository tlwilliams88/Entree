using System.Collections.Generic;
using System.Linq;

using Autofac;
using CommerceServer.Foundation;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Seams;


namespace KeithLink.Svc.Impl.Tests.Unit.Repository.Profile {
    public class UserProfileRepositoryTests : BaseDITests {
        public class CreateUserProfile {
            [Fact]
            public void WhenCreateUserProfileCalledFor_ResultingRequestToCommerceServerHasBranchPassedIn() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IUserProfileRepository testunit = MakeTestsLogic(true, ref mockDependents);
                string testCreatedBy = "Test Created By";
                string testEmail = "Test Email Address";
                string testFirstName = "Test First Name";
                string testLastName = "Test Last Name";
                string testPhonenumber = "Test Phone Number";
                string testBranch = "Test Branch";
                FoundationService.Requests = new List<CommerceRequest>();

                // act
                testunit.CreateUserProfile(testCreatedBy, testEmail, testFirstName, testLastName, testPhonenumber, testBranch);
                CommerceEntity testCSRequestModel =
                        FoundationService.Requests.First()
                                         .Operations.First()
                                         .Model;

                // assert
                testCSRequestModel.Properties["DefaultBranch"]
                                  .Should()
                                  .Be(testBranch);
            }

            [Fact]
            public void WhenCreateUserProfileCalledFor_ResultingRequestToCommerceServerHasEmailPassedIn() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IUserProfileRepository testunit = MakeTestsLogic(true, ref mockDependents);
                string testCreatedBy = "Test Created By";
                string testEmail = "Test Email Address";
                string testFirstName = "Test First Name";
                string testLastName = "Test Last Name";
                string testPhonenumber = "Test Phone Number";
                string testBranch = "Test Branch";
                FoundationService.Requests = new List<CommerceRequest>();

                // act
                testunit.CreateUserProfile(testCreatedBy, testEmail, testFirstName, testLastName, testPhonenumber, testBranch);
                CommerceEntity testCSRequestModel =
                        FoundationService.Requests.First()
                                         .Operations.First()
                                         .Model;

                // assert
                testCSRequestModel.Properties["Email"]
                                  .Should()
                                  .Be(testEmail);
            }

            [Fact]
            public void WhenCreateUserProfileCalledFor_ResultingRequestToCommerceServerHasFirstNamePassedIn() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IUserProfileRepository testunit = MakeTestsLogic(true, ref mockDependents);
                string testCreatedBy = "Test Created By";
                string testEmail = "Test Email Address";
                string testFirstName = "Test First Name";
                string testLastName = "Test Last Name";
                string testPhonenumber = "Test Phone Number";
                string testBranch = "Test Branch";
                FoundationService.Requests = new List<CommerceRequest>();

                // act
                testunit.CreateUserProfile(testCreatedBy, testEmail, testFirstName, testLastName, testPhonenumber, testBranch);
                CommerceEntity testCSRequestModel =
                        FoundationService.Requests.First()
                                         .Operations.First()
                                         .Model;

                // assert
                testCSRequestModel.Properties["FirstName"]
                                  .Should()
                                  .Be(testFirstName);
            }

            [Fact]
            public void WhenCreateUserProfileCalledFor_ResultingRequestToCommerceServerHasLastNamePassedIn() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IUserProfileRepository testunit = MakeTestsLogic(true, ref mockDependents);
                string testCreatedBy = "Test Created By";
                string testEmail = "Test Email Address";
                string testFirstName = "Test First Name";
                string testLastName = "Test Last Name";
                string testPhonenumber = "Test Phone Number";
                string testBranch = "Test Branch";
                FoundationService.Requests = new List<CommerceRequest>();

                // act
                testunit.CreateUserProfile(testCreatedBy, testEmail, testFirstName, testLastName, testPhonenumber, testBranch);
                CommerceEntity testCSRequestModel =
                        FoundationService.Requests.First()
                                         .Operations.First()
                                         .Model;

                // assert
                testCSRequestModel.Properties["LastName"]
                                  .Should()
                                  .Be(testLastName);
            }

            [Fact]
            public void WhenCreateUserProfileCalledFor_ResultingRequestToCommerceServerHasTelephonePassedIn() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IUserProfileRepository testunit = MakeTestsLogic(true, ref mockDependents);
                string testCreatedBy = "Test Created By";
                string testEmail = "Test Email Address";
                string testFirstName = "Test First Name";
                string testLastName = "Test Last Name";
                string testPhonenumber = "Test Phone Number";
                string testBranch = "Test Branch";
                FoundationService.Requests = new List<CommerceRequest>();

                // act
                testunit.CreateUserProfile(testCreatedBy, testEmail, testFirstName, testLastName, testPhonenumber, testBranch);
                CommerceEntity testCSRequestModel =
                        FoundationService.Requests.First()
                                         .Operations.First()
                                         .Model;

                // assert
                testCSRequestModel.Properties["Telephone"]
                                  .Should()
                                  .Be(testPhonenumber);
            }
        }

        #region Setup
        public class MockDependents {
            public Mock<IEventLogRepository> IEventLogRepository { get; set; }

            public Mock<IAuditLogRepository> IAuditLogRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeIEventLogRepository()
                                            .Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeIAuditLogRepository()
                                            .Object)
                  .As<IAuditLogRepository>();
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository() {
                Mock<IAuditLogRepository> mock = new Mock<IAuditLogRepository>();

                return mock;
            }
        }

        private static IUserProfileRepository MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IUserProfileRepository>();
            }
            mockDependents = new MockDependents();
            mockDependents.IEventLogRepository = MockDependents.MakeIEventLogRepository();
            mockDependents.IAuditLogRepository = MockDependents.MakeIAuditLogRepository();

            UserProfileRepository testunit = new UserProfileRepository(mockDependents.IEventLogRepository.Object, mockDependents.IAuditLogRepository.Object);
            return testunit;
        }
        #endregion
    }
}