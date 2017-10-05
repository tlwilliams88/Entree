using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.SiteCatalog {
    public class SiteCatalogListLogicTests {
        private static ICatalogLogic MakeMockLogic() {
            ContainerBuilder builder = DependencyMapFactory.GetTestsContainer();

            // Implemented
            builder.RegisterInstance(MakeMockSiteCatalogRepo())
                   .As<ICatalogRepository>();
            builder.RegisterInstance(MakeMockImageRepository())
                   .As<IProductImageRepository>();
            builder.RegisterInstance(MakeMockExportSettingLogic())
                   .As<IExportSettingLogic>();
            builder.RegisterInstance(MakeMockPriceLogic())
                   .As<IPriceLogic>();

            // Not Implemented Yet
            builder.RegisterInstance(new Mock<ICacheRepository>().Object)
                   .As<ICacheRepository>();
            builder.RegisterInstance(new Mock<IDivisionLogic>().Object)
                   .As<IDivisionLogic>();
            builder.RegisterInstance(new Mock<IFavoritesListLogic>().Object)
                   .As<IFavoritesListLogic>();
            builder.RegisterInstance(new Mock<IHistoryLogic>().Object)
                   .As<IHistoryLogic>();
            builder.RegisterInstance(new Mock<ICategoryImageRepository>().Object)
                   .As<ICategoryImageRepository>();
            builder.RegisterInstance(new Mock<IOrderHistoryDetailRepository>().Object)
                   .As<IOrderHistoryDetailRepository>();
            builder.RegisterInstance(new Mock<IOrderHistoryHeaderRepsitory>().Object)
                   .As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterInstance(new Mock<INoteLogic>().Object)
                   .As<INoteLogic>();

            IContainer dependencyMap = builder.Build();

            return dependencyMap.Resolve<ICatalogLogic>();
        }

        private static IExportSettingLogic MakeMockExportSettingLogic() {
            Mock<IExportSettingLogic> mockLogic = new Mock<IExportSettingLogic>();

            mockLogic.Setup(l => l.ReadExternalCatalogs())
                     .Returns(
                              new List<ExportExternalCatalog> {
                                  new ExportExternalCatalog {
                                      CatalogId = "FRT",
                                      Type = "FRTR",
                                      BekBranchId = "FRT?"
                                  }
                              }
                             );

            return mockLogic.Object;
        }

        private static IPriceLogic MakeMockPriceLogic() {
            Mock<IPriceLogic> mockPriceLogic = new Mock<IPriceLogic>();

            mockPriceLogic.Setup(p => p.GetPrices(
                                                  It.Is<string>(b => b == "FUT"),
                                                  It.Is<string>(c => c == "123456"),
                                                  It.IsAny<DateTime>(),
                                                  It.IsAny<List<Product>>()))
                          .Returns(
                                   new PriceReturn {
                                       Prices = new List<Price> {
                                           new Price {
                                               BranchId = "FUT",
                                               CustomerNumber = "123456",
                                               ItemNumber = "555555",
                                               CasePrice = 99.99,
                                               DeviatedCost = false
                                           }
                                       }
                                   }
                                  );

            return mockPriceLogic.Object;
        }

        private static ICatalogRepository MakeMockSiteCatalogRepo() {
            Mock<ICatalogRepository> mockRepo = new Mock<ICatalogRepository>();

            mockRepo.Setup(d => d.GetProductsByItemNumbers(It.Is<string>(p => p == "FUT"),
                                                           It.Is<List<string>>(p => p.Contains("555555")),
                                                           It.IsAny<SearchInputModel>()))
                    .Returns(new ProductsReturn {
                        Count = 1,
                        TotalCount = 1,
                        Facets = MakeFacets(),
                        Products = MakeBekProducts()
                    });

            return mockRepo.Object;
        }

        private static IProductImageRepository MakeMockImageRepository() {
            Mock<IProductImageRepository> mockRepo = new Mock<IProductImageRepository>();

            mockRepo.Setup(d => d.GetImageList(It.Is<string>(i => i.Equals("555555")),
                                               It.Is<bool>(i => i.Equals(true))))
                    .Returns(new ProductImageReturn {
                        ProductImages = new List<ProductImage> {
                            new ProductImage {
                                FileName = "TestFileName.png",
                                Height = "1080",
                                Width = "760"
                            }
                        }
                    });

            return mockRepo.Object;
        }

        /// <summary>
        ///     Returns a facets ExpandoObject based on returned values
        ///     As tests are added sections need to be filled out to match
        ///     what is retuned from ElasticSearch
        /// </summary>
        /// <returns></returns>
        private static dynamic MakeFacets() {
            dynamic facets = new ExpandoObject();

            facets.allergens = new[] {
                new {}
            };

            facets.brands = new[] {
                new {
                    brand_control_label = "test_control_label",
                    count = 1,
                    name = "BannyKeit"
                }
            };

            facets.categories = new[] {
                new {
                    categoryname = "testraunt supply",
                    code = "85",
                    count = 1,
                    name = "Testaraunt Supply"
                }
            };

            facets.dietary = new[] {
                new {}
            };

            facets.itemspecs = new[] {
                new {
                    name = "sellsheet",
                    count = 1
                }
            };

            facets.mfname = new[] {
                new {}
            };

            facets.nonstock = new[] {
                new {}
            };

            facets.parentcategories = new[] {
                new {}
            };

            facets.specialfilters = new[] {
                new {}
            };

            return facets;
        }

        private static List<Product> MakeBekProducts() {
            List<Product> products = new List<Product>();

            products.Add(new Product {
                CatalogId = "FUT",
                ItemNumber = "555555",
                CategoryCode = "85",
                CategoryName = "Restaurant Supply",
                SubCategoryCode = "8",
                ItemClass = "Equip & Supply",
                Name = "Bus Tub Lid For 15x20 Black",
                Description = "Fits Tub #918023",
                Brand = "CARLISLE",
                BrandExtendedDescription = "CARLISLE",
                BrandControlLabel = "",
                Pack = "0001",
                Size = "EA",
                UPC = "00077838098943",
                ManufacturerNumber = "4401203",
                ManufacturerName = "CARLISLE",
                Cases = "99",
                Status1 = "",
                CaseOnly = true,
                VendorItemNumber = "002277",
                Kosher = "N",
                ReplacementItem = "000000",
                ReplacedItem = "000000",
                ChildNutrition = "N",
                Nutritional = new Nutritional {
                    BrandOwner = "Carlisle Foodservice Products",
                    CountryOfOrigin = "US",
                    GrossWeight = "11.500",
                    HandlingInstructions = "N/A",
                    Ingredients = "",
                    MarketingMessage = "Comfort Curve™ Universal Bus Box Lid 20\" x 15\" x 0.75\" - Black",
                    MoreInformation = "",
                    ServingSize = "0.000",
                    ServingSizeUOM = "",
                    ServingsPerPack = "0",
                    ServingSugestion = "",
                    Shelf = "999",
                    StorageTemp = "32 FA / 212 FA",
                    UnitMeasure = "12.000",
                    UnitsPerCase = "0",
                    Volume = "0.952",
                    Height = "4.250",
                    Length = "22.125",
                    Width = "17.500",
                    NutritionInfo = null,
                    Diets = null,
                    Allergens = new Allergen {
                        freefrom = new List<string>(),
                        maycontain = new List<string>(),
                        contains = new List<string>()
                    }
                },
                NonStock = "N",
                TempZone = "D",
                CatchWeight = false,
                SellSheet = "Y",
                IsProprietary = false,
                ProprietaryCustomers = null,
                AverageWeight = 11.5
            });

            return products;
        }

        public class GetProductsByItemNumbers {
            [Fact]
            public void GoodItem_GetProductsByItemNumberReturnsProductImages() {
                // arrange
                int expected = 1;
                ICatalogLogic testLogic = MakeMockLogic();
                Mock<ICatalogRepository> testRepo = new Mock<ICatalogRepository>();
                List<string> testItemNumbers = new List<string> {
                    "555555"
                };
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                ProductsReturn results = testLogic.GetProductsByItemNumbers(testContext, testItemNumbers, new SearchInputModel(), new UserProfile());

                // assert
                results.Products
                       .First()
                       .ProductImages.Count()
                       .Should()
                       .Be(expected);
            }
        }
    }
}