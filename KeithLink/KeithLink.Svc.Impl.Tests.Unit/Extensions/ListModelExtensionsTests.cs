using FluentAssertions;

using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Extensions;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Extensions {
    public class ListModelExtensionsTests {
        [Fact]
        public void UsingListAsBranchIs_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_BRANCHIS
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsDsm_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_DSM
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsDsr_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_DSR
            };
            bool expected = false;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsGuest_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_GUEST
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsKbitAdmin_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_KBITADMIN
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsMarketing_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_MARKETING
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsPoweruser_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_POWERUSER
            };
            bool expected = true;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }

        [Fact]
        public void UsingListAsSysAdmin_SetListReadOnlyAsExpected() {
            // arrange
            ListModel list = new ListModel {
                Type = ListType.Mandatory,
                ReadOnly = true
            };
            UserProfile user = new UserProfile {
                RoleName = Constants.ROLE_NAME_SYSADMIN
            };
            bool expected = false;

            // act

            // assert - Always returns what is setup provided the mock is called
            list.SetUserSpecificProperties(user)
                .ReadOnly
                .Should()
                .Be(expected);
        }
    }
}