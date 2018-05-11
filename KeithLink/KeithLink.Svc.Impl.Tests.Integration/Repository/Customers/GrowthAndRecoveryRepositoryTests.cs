using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Customers
{
    public class GrowthAndRecoveryRepositoryTests
    {
        private static IGrowthAndRecoveriesRepository MakeRepo()
        {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IGrowthAndRecoveriesRepository>();
        }

        public class Reads : MigratedDatabaseTest {

            [Fact]
            public void GoodData_GetsGrowthAndRecoveries() {
                // arrange
                var repo = MakeRepo();
                var customerNumber = "111111";
                var branchId = "FUT";
                var expected = 3;

                // act
                List<GrowthAndRecoveriesModel> results = repo.GetGrowthAdnGetGrowthAndRecoveryOpportunities(customerNumber, branchId);

                // assert
                results.Count()
                       .Should()
                       .Be(expected);
            }

        }
    }
}
