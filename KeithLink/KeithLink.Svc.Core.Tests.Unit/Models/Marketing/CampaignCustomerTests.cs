using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using KeithLink.Svc.Core.Models.Marketing;
using Xunit;


namespace KeithLink.Svc.Core.Tests.Unit.Models.Marketing {
    public class CampaignCustomerTests {
        private static CampaignCustomer MakeCustomer() {
            return new CampaignCustomer() {
                CampaignId = 7,
                BranchId =  "Fake Branch Id",
                CustomerNumber = "Fake Customer Number"
            };
        }

        public class CampaignId {
            [Fact]
            public void GoodCustomer_HasExpectedValue() {
                // arrange
                var fake = MakeCustomer();
                var expected = 7;

                // act
                
                // assert
                fake.CampaignId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedCustomer_HasDefaultValue() {
                // arrange
                var test = new CampaignCustomer();
                var expected = 0;

                // act

                // assert
                test.CampaignId
                    .Should()
                    .Be(expected);
            }
        }

        public class BranchId {
            [Fact]
            public void GoodCustomer_HasExpectedValue() {
                // arrange
                var fake = MakeCustomer();
                var expected = "Fake Branch Id";

                // act

                // assert
                fake.BranchId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedCustomer_HasDefaultValue() {
                // arrange
                var test = new CampaignCustomer();

                // act

                // assert
                test.BranchId
                    .Should()
                    .BeNull();
            }
        }

        public class CustomerNumber {
            [Fact]
            public void GoodCustomer_HasExpectedValue() {
                // arrange
                var fake = MakeCustomer();
                var expected = "Fake Customer Number";

                // act

                // assert
                fake.CustomerNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedCustomer_HasDefaultValue() {
                // arrange
                var test = new CampaignCustomer();

                // act

                // assert
                test.CustomerNumber
                    .Should()
                    .BeNull();
            }
        }
    }
}
