using System.Dynamic;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Seams;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Repository.SiteCatelog {
    public class ElasticSearchCatalogRepositoryTests : BaseDITests {
        #region LoadProductFromElasticSearchProduct
        public class LoadProductFromElasticSearchProduct {
            [Fact]
            public void GoodItem_DetailIsExpected() {
                // arrange
                ICatalogRepository testunit = MakeTestsRepository();
                dynamic testProduct = MakeTestProduct();
                bool testListOnly = false;
                string expected = "Slush Flavor Watermelon / 102438 / DRAFT - METRO SWEET / Grocery / 6 / 64 OZ";

                // act
                Product result = testunit.LoadProductFromElasticSearchProduct(testListOnly, testProduct);

                // assert
                result.Detail.Should()
                      .Be(expected);
            }
        }
        #endregion LoadProductFromElasticSearchProduct

        #region Setup
        private static ICatalogRepository MakeTestsRepository() {
            BEKConfiguration.Add("ElasticSearchURL", "http://localhost/Test");

            ElasticSearchCatalogRepositoryImpl testunit = new ElasticSearchCatalogRepositoryImpl();
            return testunit;
        }

        private static dynamic MakeTestProduct() {
            dynamic testProduct = new ExpandoObject();
            testProduct._index = "fsa";
            testProduct._type = "product";
            testProduct._id = "102438";
            testProduct._score = 1.1391112;
            testProduct._source = new ExpandoObject();
            testProduct._source.categoryid = "46";
            testProduct._source.categoryname = "Beverages Non Dispense";
            testProduct._source.categoryname_not_analyzed = "Beverages Non Dispense";
            testProduct._source.parentcategoryid = "4";
            testProduct._source.parentcategoryname = "Grocery";
            testProduct._source.parentcategoryname_not_analyzed = "Grocery";
            testProduct._source.name = "Slush Flavor Watermelon";
            testProduct._source.name_not_analyzed = "slush flavor watermelon";
            testProduct._source.name_ngram_analyzed = "slush flavor watermelon";
            testProduct._source.description = "Rtu";
            testProduct._source.brand = "DRAFTMS";
            testProduct._source.brand_not_analyzed = "DRAFTMS";
            testProduct._source.brand_description = "DRAFT - METRO SWEET";
            testProduct._source.brand_description_not_analyzed = "DRAFT - METRO SWEET";
            testProduct._source.brand_control_label = "";
            testProduct._source.pack = "0006";
            testProduct._source.size = "64 OZ";
            testProduct._source.upc = "00000000000000";
            testProduct._source.mfrnumber = "SSWATERMEL";
            testProduct._source.mfrname = "METRO SWEET";
            testProduct._source.mfrname_not_analyzed = "METRO SWEET";
            testProduct._source.cases = "5";
            testProduct._source.package = "0";
            testProduct._source.preferreditemcode = "";
            testProduct._source.itemtype = "P";
            testProduct._source.status1 = "";
            testProduct._source.status1_not_analyzed = "";
            testProduct._source.status2 = "";
            testProduct._source.caseonly = "Y";
            testProduct._source.specialorderitem = "N";
            testProduct._source.vendor1 = "007313";
            testProduct._source.vendor2 = "000000";
            testProduct._source.itemclass = "";
            testProduct._source.department = "";
            testProduct._source.catmgr = "74";
            testProduct._source.buyer = "09";
            testProduct._source.kosher = "N";
            testProduct._source.branchid = "FSA";
            testProduct._source.replacementitem = "000000";
            testProduct._source.replaceditem = "000000";
            testProduct._source.childnutrition = "N";
            testProduct._source.itemnumber = "102438";
            testProduct._source.nutritional = null;
            testProduct._source.nonstock = "N";
            testProduct._source.itemspecification = null;
            testProduct._source.temp_zone = "D";
            testProduct._source.catchweight = false;
            testProduct._source.sellsheet = "";
            testProduct._source.isproprietary = true;
            testProduct._source.proprietarycustomers = "727299 727304 727306 727307 727313 727316 727319 727302 727309 727310 727317 727320 727325 749435 ";
            testProduct._source.averageweight = 27.27;
            testProduct._source.mfritemnumber = null;
            testProduct._source.warehousenumber = null;
            testProduct._source.clength = null;
            testProduct._source.cwidth = null;
            testProduct._source.cheight = null;
            testProduct._source.unitofsale = null;
            testProduct._source.catalogdept = null;
            testProduct._source.shipminexpire = null;
            testProduct._source.minorder = null;
            testProduct._source.vendorname = null;
            testProduct._source.ti = null;
            testProduct._source.hi = null;
            testProduct._source.palt = null;
            testProduct._source.casequantity = null;
            testProduct._source.putup = null;
            testProduct._source.contsize = null;
            testProduct._source.contunit = null;
            testProduct._source.tcscode = null;
            testProduct._source.caseupc = null;
            testProduct._source.plength = null;
            testProduct._source.pheight = null;
            testProduct._source.pwidth = null;
            testProduct._source.status = null;
            testProduct._source.flag1 = null;
            testProduct._source.flag2 = null;
            testProduct._source.flag3 = null;
            testProduct._source.flag4 = null;
            testProduct._source.onhandqty = null;
            testProduct._source.caseprice = null;
            testProduct._source.packageprice = null;
            testProduct._source.stockedinbranches = null;
            testProduct._source.marketing_brand = null;
            testProduct._source.marketing_brand_not_analyzed = null;
            testProduct._source.marketing_description = null;
            testProduct._source.marketing_manufacturer = null;
            testProduct._source.marketing_manufacturer_not_analyzed = null;
            testProduct._source.marketing_name = null;
            testProduct._source.marketing_name_not_analyzed = null;
            testProduct._source.marketing_name_ngram_analyzed = null;
            return testProduct;
        }
        #endregion Setup
    }
}