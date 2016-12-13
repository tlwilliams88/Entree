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
using KeithLink.Svc.WebApi.Models;
using KeithLink.Common.Core.Interfaces.Logging;

namespace KeithLink.Svc.WebApi.Controllers
{
	/// <summary>
	/// House Brands
	/// </summary>
    public class BrandController : BaseController {
        #region attributes
        KeithLink.Svc.Core.Interface.Brand.IBrandRepository _brandRepository;
        private readonly IEventLogRepository _elRepo;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brandRepository"></param>
        /// <param name="profileLogic"></param>
        /// <param name="elRepo"></param>
        public BrandController(IBrandRepository brandRepository, IUserProfileLogic profileLogic, IEventLogRepository elRepo) : base(profileLogic) {
            _brandRepository = brandRepository;
            this._elRepo = elRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a list of house brands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("brands/house")]
        public OperationReturnModel<BrandsReturn> GetHouseBrands()
        {
            OperationReturnModel<BrandsReturn> ret = new OperationReturnModel<BrandsReturn>();
            try
            {
                ret.SuccessResponse = _brandRepository.GetHouseBrands();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetHouseBrands", ex);
            }
            return ret;
        }
        #endregion
    } 
}