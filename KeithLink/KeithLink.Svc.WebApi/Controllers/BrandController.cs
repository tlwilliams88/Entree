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
        /// constructor
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

                // This is because it wasn't worth the effort of redesigning the system to satisfy Marketing request
                // Basically we are combinine the Markon brand control labels into one.
                var markonBrand = ret.SuccessResponse.Brands.First(x => x.BrandControlLabel.Equals("MA"));

                // Remove the brand to re-order it to the first in the list
                ret.SuccessResponse.Brands.Remove(markonBrand);

                // Set the markon brand to pull back all brand control labels
                markonBrand.BrandControlLabel = "MA ME MC MR";

                // Add the markon brand to the first item in the list
                ret.SuccessResponse.Brands.Insert(0, markonBrand);

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