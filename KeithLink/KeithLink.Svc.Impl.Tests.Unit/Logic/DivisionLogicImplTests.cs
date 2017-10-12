using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic
{
    public class DivisionLogicImplTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<IDivisionRepository> DivisionRepository { get; set; }

            public Mock<IBranchSupportRepository> BranchSupportRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeDivisionRepositoryMock().Object)
                  .As<IDivisionRepository>();
                cb.RegisterInstance(MakeBranchSupportRepositoryMock().Object)
                  .As<IBranchSupportRepository>();
            }

            public static Mock<IDivisionRepository> MakeDivisionRepositoryMock()
            {
                var mock = new Mock<IDivisionRepository>();

                mock.Setup(x => x.GetDivisions())
                    .Returns(
                             new List<Catalog>() {
                                 new Catalog() {
                                     Id = "FUT",
                                     DisplayName = "FUTBALL"
                                 },
                                 new Catalog() {
                                     Id = "FOT",
                                     DisplayName = "FOTBULL"
                                 }
                             }
                            );

                return mock;
            }

            public static Mock<IBranchSupportRepository> MakeBranchSupportRepositoryMock()
            {
                var mock = new Mock<IBranchSupportRepository>();

                mock.Setup(x => x.ReadAll())
                    .Returns(
                             new List<BranchSupport>() {
                                 new BranchSupport() {
                                     BranchId = "FUT",
                                     BranchName = "FUTBALL NAME",
                                     Email = "fut@ball.net",
                                     SupportPhoneNumber = "888-888-8888",
                                     TollFreeNumber = "999-999-9999"
                                 }
                             }
                            );

                return mock;
            }

        }

        private static IDivisionLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IDivisionLogic>();
            }
            else
            {
                mockDependents = new MockDependents();
                mockDependents.DivisionRepository = MockDependents.MakeDivisionRepositoryMock();
                mockDependents.BranchSupportRepository = MockDependents.MakeBranchSupportRepositoryMock();

                var testunit = new DivisionLogicImpl(mockDependents.DivisionRepository.Object, mockDependents.BranchSupportRepository.Object);
                return testunit;
            }
        }
        #endregion

        #region attributes
        #endregion

        #region GetDivisions

        public class GetDivisions {
            [Fact]
            public void WhenGetDivisionsIsCalled_MatchesBranchSupportBranchNameWithDivision() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac:true, mockDependents: ref mockDependents);
                var branchId = "FUT";
                var expected = "FUTBALL NAME";

                // act

                // assert
                testunit.GetDivisions()
                        .First(y => y.Id.Equals(branchId))
                        .BranchSupport.BranchName
                        .Should()
                        .Be(expected);
            }
        }
        #endregion
    }
}
