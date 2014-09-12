using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core;
using System.Web.Http.Cors;
using System.Dynamic;
using KeithLink.Svc.Core.Models.Brand;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class BrandController : BaseController
    {
        KeithLink.Svc.Core.Interface.Brand.IBrandRepository _brandRepository;

        public BrandController(IBrandRepository brandRepository, IUserProfileRepository userRepo)
            : base(userRepo)
        {
            _brandRepository = brandRepository;
        }

        // GET api/<controller>
        [HttpGet]
        [ApiKeyedRoute("brands/house")]
        public BrandsReturn GetHouseBrands()
        {
            return _brandRepository.GetHouseBrands();
        }




    } // end class
} // end namespace