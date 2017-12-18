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

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.SiteCatalog
{
    public class SiteCatalogListLogicTests
    {
        private static ICatalogLogic MakeMockLogic()
        {
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
            builder.RegisterInstance(makeMockCacheRepository())
                .As<ICacheRepository>();
            builder.RegisterInstance(makeMockCategoryImageRepository())
                .As<ICategoryImageRepository>();

            // Not Implemented Yet
            builder.RegisterInstance(new Mock<IDivisionLogic>().Object)
                .As<IDivisionLogic>();
            builder.RegisterInstance(new Mock<IFavoritesListLogic>().Object)
                .As<IFavoritesListLogic>();
            builder.RegisterInstance(new Mock<IHistoryLogic>().Object)
                .As<IHistoryLogic>();
            builder.RegisterInstance(new Mock<IOrderHistoryDetailRepository>().Object)
                .As<IOrderHistoryDetailRepository>();
            builder.RegisterInstance(new Mock<IOrderHistoryHeaderRepsitory>().Object)
                   .As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterInstance(new Mock<INoteLogic>().Object)
                   .As<INoteLogic>();


            IContainer dependencyMap = builder.Build();

            return dependencyMap.Resolve<ICatalogLogic>();
        }

        private static IExportSettingLogic MakeMockExportSettingLogic()
        {
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

        private static IPriceLogic MakeMockPriceLogic()
        {
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

        private static ICatalogRepository MakeMockSiteCatalogRepo()
        {
            Mock<ICatalogRepository> mockRepo = new Mock<ICatalogRepository>();

            mockRepo.Setup(d => d.GetProductsByItemNumbers(
                    It.Is<string>(p => p == "FUT"),
                    It.Is<List<string>>(p => p.Contains("555555")),
                    It.IsAny<SearchInputModel>()))
                .Returns(new ProductsReturn {
                    Count = 1,
                    TotalCount = 1,
                    Facets = MakeFacets(),
                    Products = MakeBekProducts()
                });

            mockRepo.Setup(d => d.GetHitsForSearchInIndex(
                    It.IsAny<UserSelectedContext>(),
                    It.IsAny<string>(),
                    It.IsAny<SearchInputModel>()))
                .Returns(10);

            mockRepo.Setup(d => d.GetCategories(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
                .Returns(new CategoriesReturn {
                    Categories = new List<Category>()
                    {
                        new Category()
                        {
                            Id = "123456",
                            Name = "Test, Category/1 & Name.",
                            SearchName = "TestSearchName1",
                            Description = "Test Description of Category 1",
                            CategoryImage = null,
                            SubCategories = new SubCategory[]
                            {
                                new SubCategory()
                                {
                                    Id = "654321",
                                    Name = "Test, SubCategory/1 & Name.",
                                    SearchName = "TestSubCategory 1",
                                    Description = "Test Description of SubCategory 1",
                                    PPICode = "1234567890"
                                }
                            },
                            Department = "Department1"
                        },
                        new Category()
                        {
                            Id = "234567",
                            Name = "Test, Category/2 & Name.",
                            SearchName = "TestSearchName2",
                            Description = "Test Description of Category 2",
                            CategoryImage = new CategoryImage()
                            {
                                FileName = "category2.test",
                                Url = "category2.test.com",
                                Width = "100px",
                                Height = "100px"
                            },
                            SubCategories = new SubCategory[]
                            {
                                new SubCategory()
                                {
                                    Id = "765432",
                                    Name = "Test, SubCategory/2 & Name.",
                                    SearchName = "TestSubCategory 2",
                                    Description = "Test Description of SubCategory 2",
                                    PPICode = "2345678901"
                                }
                            },
                            Department = "Department2"
                        }
                    }
                });

            mockRepo.Setup(d => d.GetProductsByIds(
                    It.IsAny<string>(),
                    It.IsAny<List<string>>()))
                .Returns(new ProductsReturn {
                    Count = 1,
                    TotalCount = 1,
                    Facets = MakeFacets(),
                    Products = MakeBekProducts()
                });

            mockRepo.Setup(d => d.SeekSpecialFilters(It.IsAny<string>()))
                .Returns(new List<string>()
                {
                    "testFacet"
                });

            mockRepo.Setup(d => d.SeekSpecialFilters(It.Is<string>(s => s == "specialFilter:test")))
                .Returns(new List<string>()
                {
                    "deviatedprices"
                });

            mockRepo.Setup(d => d.GetProductsBySearch(
                    It.IsAny<UserSelectedContext>(),
                    It.IsAny<string>(),
                    It.IsAny<SearchInputModel>()))
                .Returns(new ProductsReturn() {
                    Count = 1,
                    TotalCount = 1,
                    Facets = MakeFacets(),
                    Products = MakeBekProducts()
                });

            mockRepo.Setup(d => d.GetProductsBySearch(
                    It.IsAny<UserSelectedContext>(),
                    It.IsAny<string>(),
                    It.Is<SearchInputModel>(s => s.Size > 1)))
                .Returns(new ProductsReturn() {
                    Count = 2,
                    TotalCount = 2,
                    Facets = MakeFacets(),
                    Products = MakeBekProducts()
                });

            return mockRepo.Object;
        }

        private static IProductImageRepository MakeMockImageRepository()
        {
            Mock<IProductImageRepository> mockRepo = new Mock<IProductImageRepository>();

            mockRepo.Setup(d => d.GetImageList(
                    It.Is<string>(i => i.Equals("555555")),
                    It.Is<bool>(i => i.Equals(true))))
                .Returns(new ProductImageReturn() {
                    ProductImages = new List<ProductImage>()
                    {
                        new ProductImage()
                        {
                            FileName = "TestFileName.png",
                            Height = "1080",
                            Width = "760"
                        }
                    }
                });

            return mockRepo.Object;
        }

        private static ICacheRepository makeMockCacheRepository()
        {
            Mock<ICacheRepository> mockCacheRepo = new Mock<ICacheRepository>();

            mockCacheRepo.Setup(func => func.GetItem<CategoriesReturn>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new CategoriesReturn {
                    Categories = new List<Category>()
                    {
                        new Category()
                        {
                            Id = "123456",
                            Name = "Test, Cache Category/1 & Name.",
                            SearchName = "TestCacheSearchName1",
                            Description = "Test Cache Description of Category 1",
                            CategoryImage = new CategoryImage()
                            {
                                FileName = "cache.category1.test",
                                Url = "cache.category1.test.com",
                                Width = "100px",
                                Height = "100px"
                            },
                            SubCategories = new SubCategory[]
                            {
                                new SubCategory()
                                {
                                    Id = "654321",
                                    Name = "Test, CacheSubCategory/1 & Name.",
                                    SearchName = "TestCacheSubCategory 1",
                                    Description = "Test Description of CacheSubCategory 1",
                                    PPICode = "1234567890"
                                }
                            },
                            Department = "CacheDepartment1"
                        },
                        new Category()
                        {
                            Id = "234567",
                            Name = "Test, CachceCategory/2 & Name.",
                            SearchName = "TestCacheSearchName2",
                            Description = "Test Cache Description of Category 2",
                            CategoryImage = new CategoryImage()
                            {
                                FileName = "cache.category2.test",
                                Url = "cache.category2.test.com",
                                Width = "100px",
                                Height = "100px"
                            },
                            SubCategories = new SubCategory[]
                            {
                                new SubCategory()
                                {
                                    Id = "765432",
                                    Name = "Test, CacheSubCategory/2 & Name.",
                                    SearchName = "TestCacheSubCategory 2",
                                    Description = "Test Cache Description of SubCategory 2",
                                    PPICode = "2345678901"
                                }
                            },
                            Department = "CacheDepartment2"
                        }
                    }
                });
            mockCacheRepo.Setup(func => func.GetItem<CategoriesReturn>(
                    It.Is<string>(nm => nm != "Catalog"),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns((CategoriesReturn)null);

            mockCacheRepo.Setup(func => func.AddItem(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CategoriesReturn>()));

            return mockCacheRepo.Object;
        }

        private static ICategoryImageRepository makeMockCategoryImageRepository()
        {
            Mock<ICategoryImageRepository> mockImageRepo = new Mock<ICategoryImageRepository>();

            mockImageRepo.Setup(func => func.GetImageByCategory(It.IsAny<string>()))
                .Returns(new CategoryImageReturn() {
                    CategoryImage = new CategoryImage() {
                        FileName = "category.test",
                        Url = "category.test.com",
                        Width = "100px",
                        Height = "100px"
                    }
                });

            return mockImageRepo.Object;
        }

        /// <summary>
        ///     Returns a facets ExpandoObject based on returned values
        ///     As tests are added sections need to be filled out to match
        ///     what is retuned from ElasticSearch
        /// </summary>
        /// <returns></returns>
        private static dynamic MakeFacets()
        {
            dynamic facets = new ExpandoObject();

            facets.allergens = new[] {
                new {}
            };

            facets.brands = new[]
            {
                new
                {
                    brand_control_label = "test_control_label",
                    count = 1,
                    name = "BannyKeit"
                }
            };

            facets.categories = new[]
            {
                new
                {
                    categoryname = "testraunt supply",
                    code = "85",
                    count = 1,
                    name = "Testaraunt Supply"
                }
            };

            facets.dietary = new[] {
                new {}
            };

            facets.itemspecs = new[]
            {
                new
                {
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

        private static List<Product> MakeBekProducts()
        {
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

                Nutritional = new Nutritional() {
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

                    Allergens = new Allergen() {
                        freefrom = new List<string>() { },
                        maycontain = new List<string>() { },
                        contains = new List<string>() { }
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

        public class GetProductsByItemNumbers
        {
            [Fact]
            public void GoodItem_GetProductsByItemNumberReturnsProductImages()
            {
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
                ProductsReturn results = testLogic.GetProductsByItemNumbers(testContext, testItemNumbers,
                    new SearchInputModel(), new UserProfile());

                // assert
                results.Products
                    .First()
                    .ProductImages.Count()
                    .Should()
                    .Be(expected);
            }
        }

        public class GetCategoryName
        {
            [Fact]
            public void GoodCategories_GetCategoryNameReturnsCachedCategories()
            {
                // arrange
                ICatalogLogic testLogic = MakeMockLogic();

                // act
                CategoriesReturn results = testLogic.GetCategories(0, 2000, "testType");

                // assert
                results.Categories
                    .Count()
                    .Should()
                    .Be(2);
            }
        }

        public class GetBranchId
        {

            [Fact]
            public void NotBekBranchId_GetBranchIdReturnsSameBranchId()
            {
                // arrange
                ICatalogLogic testLogic = MakeMockLogic();
                String branchId = "fdf";
                String catalogType = "FRTR";

                // act
                string results = testLogic.GetBranchId(branchId, catalogType);

                // assert
                results.Should().Be(branchId);
            }

            [Fact]
            public void BekBranchId_GetBranchIdReturnsSameBranchId()
            {
                // arrange
                ICatalogLogic testLogic = MakeMockLogic();
                String branchId = "bek";
                String catalogType = "FRTR";

                // act
                string results = testLogic.GetBranchId(branchId, catalogType);

                // assert
                results.Should().Be(branchId);
            }

        }

        public class AddProductImageInfo
        {

            [Fact]
            public void NotBekBranchId_GetBranchIdReturnsSameBranchId()
            {
                // arrange
                ICatalogLogic testLogic = MakeMockLogic();
                String branchId = "FRT";
                String catalogType = "FRTR";

                // act
                string results = testLogic.GetBranchId(branchId, catalogType);

                // assert
                results.Should().Be(branchId);
            }

        }

        public class GetHitsForCatalogs
        {

            [Fact]
            public void NotBekBranchId_GetBranchIdReturnsSameBranchId()
            {
                // arrange
                ICatalogLogic testLogic = MakeMockLogic();
                UserSelectedContext catalogInfo = new UserSelectedContext() {
                    BranchId = "FRT?",
                    CustomerId = "123456"
                };
                string search = "hotdog";
                SearchInputModel searchModel = new SearchInputModel {
                    From = 1,
                    Size = 1,
                    Facets = "testfacet",
                    SField = "testSField",
                    SDir = "testSDir",
                    CatalogType = "testCatalogType",
                    Dept = "testDept"
                };

                Dictionary<string, int> expected = new Dictionary<string, int>();
                expected.Add("FRT?", 10);
                expected.Add("BEK", 10);
                // act
                Dictionary<string, int> results = testLogic.GetHitsForCatalogs(catalogInfo, search, searchModel);

                // assert
                results.Count.Should().Be(2);

            }

            public class IsSpecialtyCatalog
            {
                // does one branch id match return true
                // does null branchid return false
                // does different branch id return false (i.e. no matches)

                [Fact]
                public void BekBranchId_IsSpecialtyCatalogReturnsFalse()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branchId = "FRTR";
                    String catalogType = "bek";

                    // act
                    bool results = testLogic.IsSpecialtyCatalog(catalogType, branchId);

                    // assert
                    results.Should().Be(false);
                }

                [Fact]
                public void NotBekBranchId_IsSpecialtyCatalogReturnsTrue()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branchId = "_bek";
                    String catalogType = "FRTR";

                    // act
                    bool results = testLogic.IsSpecialtyCatalog(catalogType, branchId);

                    // assert
                    results.Should().Be(true);
                }

                [Fact]
                public void NullCatalogType_IsSpecialtyCatalogReturnsTrues()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branchId = "FRT";
                    String catalogType = null;

                    // act
                    bool results = testLogic.IsSpecialtyCatalog(catalogType, branchId);

                    // assert
                    results.Should().Be(true);
                }

                // branch id null return false
                [Fact]
                public void NullCatalogTypeAndBranchId_IsSpecialtyCatalogReturnsFalse()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branchId = null;
                    String catalogType = null;

                    // act
                    bool results = testLogic.IsSpecialtyCatalog(catalogType, branchId);

                    // assert
                    results.Should().Be(false);
                }

            }

            public class IsCatalogIdBEK
            {

                [Fact]
                public void GoodCatalogId_ReturnsTrue()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String catalogId = "FRT?";

                    // act
                    bool results = testLogic.IsCatalogIdBEK(catalogId);

                    // assert
                    results.Should().Be(true);
                }

                [Fact]
                public void BadCatalogId_ReturnsFalse()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String catalogId = "bek";

                    // act
                    bool results = testLogic.IsCatalogIdBEK(catalogId);

                    // assert
                    results.Should().Be(false);
                }
            }

            public class GetProductsByIds
            {

                [Fact]
                public void GoodBranch_ReturnsProducts()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branch = "FRT?";
                    List<string> ids = new List<string>()
                    {
                        "123456",
                        "234567",
                        "345678"
                    };

                    // act
                    ProductsReturn results = testLogic.GetProductsByIds(branch, ids);

                    // assert
                    results.Products.Count().Should().Be(2);
                }

                [Fact]
                public void EmptyIds_ReturnsNoProducts()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    String branch = "FRT?";
                    List<string> ids = new List<string>();

                    // act
                    ProductsReturn results = testLogic.GetProductsByIds(branch, ids);

                    // assert
                    results.Products.Count().Should().Be(0);
                }
            }

            public class GetProductsByIdsWithPricing
            {

                [Fact]
                public void GoodUserContext_ReturnsProducts()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    UserSelectedContext testContext = new UserSelectedContext() {
                        BranchId = "FUT",
                        CustomerId = "123456"
                    };
                    List<string> ids = new List<string>()
                    {
                        "123456",
                        "234567",
                        "345678"
                    };

                    // act
                    ProductsReturn results = testLogic.GetProductsByIdsWithPricing(testContext, ids);

                    // assert
                    results.Products.Count().Should().Be(1);
                }

                [Fact]
                public void EmptyIds_ReturnsNoProducts()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    UserSelectedContext testContext = new UserSelectedContext() {
                        BranchId = "FUT",
                        CustomerId = "123456"
                    };
                    List<string> ids = new List<string>();

                    // act
                    ProductsReturn results = testLogic.GetProductsByIdsWithPricing(testContext, ids);

                    // assert
                    results.Products.Count().Should().Be(0);
                }
            }

            public class GetProductsBySearch
            {

                [Fact]
                public void GoodParams_ReturnsProducts()
                {
                    // arrange
                    ICatalogLogic testLogic = MakeMockLogic();
                    UserSelectedContext testContext = new UserSelectedContext() {
                        BranchId = "FUT",
                        CustomerId = "123456"
                    };
                    String search = "hotdog";
                    SearchInputModel searchModel = new SearchInputModel() {
                        From = 0,
                        Size = 1,
                        Facets = "",
                        SField = "",
                        SDir = "",
                        CatalogType = "",
                        Dept = ""
                    };

                    // act
                    ProductsReturn results = testLogic.GetProductsBySearch(testContext, search, searchModel, null);

                    // assert
                    results.Products.Count().Should().Be(1);
                }

            }

        }
    }
}