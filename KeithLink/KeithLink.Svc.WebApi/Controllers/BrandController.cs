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
    public class BrandController : BaseController {
        #region attributes
        KeithLink.Svc.Core.Interface.Brand.IBrandRepository _brandRepository;
        #endregion

        #region ctor
        public BrandController(IBrandRepository brandRepository, IUserProfileLogic profileLogic) : base(profileLogic) {
            _brandRepository = brandRepository;
        }
        #endregion

        #region methods
        [HttpGet]
        [ApiKeyedRoute("brands/house")]
        public BrandsReturn GetHouseBrands()
        {
            return _brandRepository.GetHouseBrands();
        }
        #endregion
    } 
}