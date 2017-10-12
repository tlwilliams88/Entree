using FluentAssertions;

using KeithLink.Svc.Core.Models.SiteCatalog;

using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.SiteCatalog
{
    public class BranchSupportModelTests
    {
        #region setup
        private static BranchSupportModel MakeTestData()
        {
            return new BranchSupportModel()
            {
                BranchId = "FUT",
                BranchName = "FUTBALL",
                SupportPhoneNumber = "888-888-8888",
                Email = "fut@ball.net",
                TollFreeNumber = "999-999-9999"
            };
        }
        #endregion

        #region Get_BranchId
        public class Get_BranchId {
            [Fact]
            public void GoodTest_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "FUT";

                // act

                // assert
                fakeItem.BranchId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedTest_HasDefaultValue()
            {
                // arrange
                var test = new BranchSupportModel();

                // act

                // assert
                test.BranchId
                    .Should()
                    .BeNullOrEmpty();

            }
        }
        #endregion

        #region Get_BranchName
        public class Get_BranchName {
            [Fact]
            public void GoodTest_ReturnsExpectedValue() {
                // arrange
                var fakeData = MakeTestData();
                var expected = "FUTBALL";

                // act

                // assert
                fakeData.BranchName
                        .Should()
                        .Be(expected);

            }

            [Fact]
            public void InitializedTest_HasDefaultValue() {
                // arrange
                var test = new BranchSupportModel();
                
                // act

                // assert
                test.BranchName
                    .Should()
                    .BeNullOrEmpty();

            }
        }
        #endregion

        #region Get_SupportPhoneNumber
        public class Get_SupportPhoneNumber {
            [Fact]
            public void GoodTest_ReturnsExpectedValue() {
                // arrange
                var fakeData = MakeTestData();
                var expected = "888-888-8888";

                // act

                // assert
                fakeData.SupportPhoneNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedTest_HasDefaultValue() {
                // arrange
                var test = new BranchSupportModel();
                
                // act

                // assert
                test.SupportPhoneNumber
                    .Should()
                    .BeNullOrEmpty();

            }
        }
        #endregion

        #region Get_Email
        public class Get_Email {
            [Fact]
            public void GoodTest_ReturnsExpectedValue() {
                // arrange
                var fakeData = MakeTestData();
                var expected = "fut@ball.net";

                // act

                // assert
                fakeData.Email
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedTest_HasDefaultValue() {
                // arrange
                var test = new BranchSupportModel();
                
                // act

                // assert
                test.Email
                    .Should()
                    .BeNullOrEmpty();

            }
        }
        #endregion

        #region Get_TollFreeNumber
        public class Get_TollFreeNumber {
            [Fact]
            public void GoodTest_ReturnsExpectedValue() {
                // arrange
                var fakeData = MakeTestData();
                var expected = "999-999-9999";

                // act

                // assert
                fakeData.TollFreeNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedTest_HasDefaultValue() {
                // arrange
                var test = new BranchSupportModel();
                
                // act

                // assert
                test.TollFreeNumber
                    .Should()
                    .BeNullOrEmpty();

            }
        }
        #endregion
    }
}
