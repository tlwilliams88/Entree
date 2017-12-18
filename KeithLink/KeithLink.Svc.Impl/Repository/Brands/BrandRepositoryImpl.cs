using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Models.Brand;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl.Repository.Brands
{
    public class BrandRepositoryImpl : IBrandRepository
    {
        #region " attributes "

        private Helpers.ElasticSearch _esHelper;

        #endregion

        #region " constructor "
        public BrandRepositoryImpl()
        {
            _esHelper = new Helpers.ElasticSearch();
        }
        #endregion 

        #region " methods / functions "

        public BrandsReturn GetHouseBrands()
        {

            var response = _esHelper.ElasticClient.Search<Brand>(s => s
                .From(0)
                .Size(50)
                .Type(Constants.ES_TYPE_BRAND)
                .Index(Constants.ES_INDEX_BRANDS)
                );

            foreach (Brand r in response.Documents)
            {
                r.ImageURL = String.Format("{0}/{1}.jpg", Configuration.BrandAssetsUrl, r.BrandControlLabel.ToLower());
            }

            List<string> excludedBrands = Configuration.BrandsToExclude;

            List<Brand> filteredBrands = (from brand in response.Documents.ToList<Brand>()
                                          where !(from b in excludedBrands
                                                  select b.ToString()).Contains(brand.BrandControlLabel, StringComparer.InvariantCultureIgnoreCase)
                                          select brand).ToList<Brand>();

            BrandsReturn results = new BrandsReturn { Brands = filteredBrands };
            return results;
        }

        #endregion

        #region " properties "
        #endregion
    }
}
