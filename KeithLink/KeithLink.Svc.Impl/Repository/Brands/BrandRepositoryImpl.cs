using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Models.Brand;

namespace KeithLink.Svc.Impl.Repository.Brands
{
    public class BrandRepositoryImpl : IBrandRepository
    {
        #region " attributes "
        #endregion

        #region " constructor "
        #endregion 

        #region " methods / functions "

        public BrandsReturn GetHouseBrands()
        {
            BrandsReturn results = new BrandsReturn();

            results.Add(new Brand { Id = "AA", Name = "Markon Cooprative", ImageURL = "http://devkeithlink.bekco.com/assets/brands/markon-logo-products.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Admiral of the Fleet", ImageURL = "http://devkeithlink.bekco.com/assets/brands/admiral-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Ceylon Tea Gardens", ImageURL = "http://devkeithlink.bekco.com/assets/brands/ceylon-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "ComSource", ImageURL = "http://devkeithlink.bekco.com/assets/brands/GranSabor_sm.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Cortona", ImageURL = "http://devkeithlink.bekco.com/assets/brands/cortona-brand-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Ellington Farms", ImageURL = "http://devkeithlink.bekco.com/assets/brands/ellington-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Fresh from Keith", ImageURL = "http://devkeithlink.bekco.com/assets/brands/freshfromkeith-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Golden Harvest", ImageURL = "http://devkeithlink.bekco.com/assets/brands/golden-harvest-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith's Premium", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keith-premium-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith's Exclusive", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keith-exclusive-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith's Choice", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keith-choice-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith's Homestyle", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keith-homestyle-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith's Essentials", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keith-essential-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Keith Valley", ImageURL = "http://devkeithlink.bekco.com/assets/brands/keithvally-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Sweet D' Lite", ImageURL = "http://devkeithlink.bekco.com/assets/brands/sweet-lite-logo.jpg" });
            results.Add(new Brand { Id = "AA", Name = "BEKCO", ImageURL = "http://devkeithlink.bekco.com/assets/brands/bekco.jpg" });
            results.Add(new Brand { Id = "AA", Name = "World Horizons", ImageURL = "http://devkeithlink.bekco.com/assets/brands/bekco.jpg" });
            results.Add(new Brand { Id = "AA", Name = "Winn Meats", ImageURL = "http://devkeithlink.bekco.com/assets/brands/bekco.jpg" });

            return results;
        }

        #endregion

        #region " properties "
        #endregion
    }
}
