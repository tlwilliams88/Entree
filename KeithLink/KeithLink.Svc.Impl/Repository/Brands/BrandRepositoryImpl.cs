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
           
            var response = _esHelper.Client.Search<Brand>(s => s
                .From(0)
                .Size(50)
                .Type(Constants.ES_TYPE_BRAND)
                .Index(Constants.ES_INDEX_BRANDS)
                );


            foreach (Brand r in response.Documents)
            {
                r.ImageURL = String.Format("{0}/{1}.jpg", Configuration.BrandAssetsUrl, r.BrandControlLabel.ToLower());
            }

            BrandsReturn results = new BrandsReturn { Brands = response.Documents.ToList<Brand>() };
            return results;
        }

        #endregion

        #region " properties "
        #endregion
    }
}
