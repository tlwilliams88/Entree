using System.Collections.Generic;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;


namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class ContractChangesRepositoryTests {
        private static IContractChangesRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();

            return diMap.Resolve<IContractChangesRepository>();
        }

        public class GetContractChanges : MigratedDatabaseTest {
            [Fact]
            public void CallPurge_Completes() {
                // arrange
                IContractChangesRepository repo = MakeRepo();

                // act
                try {
                    repo.Purge(7);
                // assert
                    Assert.True(true); //no exception
                }
                catch
                {
                    Assert.True(false);//exception
                }
            }

            [Fact]
            public void CallReadNextSet_ReturnsExpectedRecords() {
                // arrange
                IContractChangesRepository repo = MakeRepo();
                int expected = 3;

                // act
                List<ContractChange> results = repo.ReadNextSet();

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void CallUpdate_Completes() {
                // arrange
                IContractChangesRepository repo = MakeRepo();
                int testId = 1;
                bool testSent = true;
                // act
                try {
                    repo.Update(testId, testSent);
                // assert
                    Assert.True(true); //no exception
                }
                catch
                {
                    Assert.True(false);//exception
                }
            }
        }
    }
}