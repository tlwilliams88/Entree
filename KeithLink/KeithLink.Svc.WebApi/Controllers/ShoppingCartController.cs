// KeithLink
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

// Core
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Common.Impl.Email;
using KeithLink.Svc.Core.Enumerations.List;

using Newtonsoft.Json;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// end points for the shopping cart
    /// </summary>
	[Authorize]
	public class ShoppingCartController : BaseController
	{
		#region attributes
        private readonly IUserActiveCartLogic _activeCartLogic;
		private readonly IShoppingCartLogic _shoppingCartLogic;
        private readonly IExportSettingLogic _exportLogic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="shoppingCartLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        /// <param name="userActiveCartLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        public ShoppingCartController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic, IEventLogRepository logRepo, 
                                      IUserActiveCartLogic userActiveCartLogic, IExportSettingLogic exportSettingsLogic) : base(profileLogic) {
            _activeCartLogic = userActiveCartLogic;
			_shoppingCartLogic = shoppingCartLogic;
            _exportLogic = exportSettingsLogic;
            _log = logRepo;
        }
        #endregion

        #region methods

        /// <summary>
        /// An endpoint for determining whether a cart submission has been made
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("cart/issubmitted/{cartId}")]
        public Models.OperationReturnModel<bool> IsSubmitted(Guid cartId)
        {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.IsSubmitted(this.AuthenticatedUser, this.SelectedUserContext, cartId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("IsSubmitted", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve user shopping carts (orders not yet submitted)
        /// </summary>
        /// <param name="header">Header level info only?</param>
        /// <returns></returns>
        [HttpGet]
		[ApiKeyedRoute("cart/")]
		public Models.OperationReturnModel<List<ShoppingCart>> List(bool header = false)
		{
            Models.OperationReturnModel<List<ShoppingCart>> retVal = new Models.OperationReturnModel<List<ShoppingCart>>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.ReadAllCarts(this.AuthenticatedUser, this.SelectedUserContext, header);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Cart List", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve a specific cart
        /// </summary>
        /// <param name="cartId">Cart id</param>
        /// <returns></returns>
        [HttpGet]
		[ApiKeyedRoute("cart/{cartId}")]
		public Models.OperationReturnModel<ShoppingCart> Cart(Guid cartId)
		{
            Models.OperationReturnModel<ShoppingCart> retVal = new Models.OperationReturnModel<ShoppingCart>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.ReadCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetCart", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }


        /// <summary>
        /// Export Cart + List
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="listId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute( "cart/print/{cartId}/{listtype}/{listId}" )]
        public HttpResponseMessage PrintCartWithList( Guid cartId, ListType listType, long listId, PrintListModel options ) {
            HttpResponseMessage ret;
            try
            { //TODO: Unravel this cartreport list...
                Stream stream = _shoppingCartLogic.CartReport
                    (AuthenticatedUser, SelectedUserContext, cartId, listId, options);

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                ret = result;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("List Export", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }

        /// <summary>
        /// Create a user cart
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("cart/")]
		public Models.OperationReturnModel<NewCSItem> Cart(ShoppingCart cart)
		{
            Models.OperationReturnModel<NewCSItem> retVal = new Models.OperationReturnModel<NewCSItem>();
            try
            {
                retVal.SuccessResponse = new NewCSItem() { Id = _shoppingCartLogic.CreateCart(this.AuthenticatedUser, this.SelectedUserContext, cart) };
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("POST Cart({0})", JsonConvert.SerializeObject(cart)), ex);
                ExceptionEmail.Send(ex, string.Format("POST Cart({0})", JsonConvert.SerializeObject(cart)));
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Export a specific cart
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="exportRequest">Export options</param>
        /// <returns> </returns>
        [HttpPost]
        [ApiKeyedRoute("cart/export/{cartId}")]
        public HttpResponseMessage ExportCartDetail(Guid cartId, ExportRequestModel exportRequest)
        {
            HttpResponseMessage ret;
            try
            {
                var cart = _shoppingCartLogic.ReadCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.CartDetail, Core.Enumerations.List.ListType.Custom,
                                                        exportRequest.Fields, exportRequest.SelectedType);

                ret = ExportModel<ShoppingCartItem>(cart.Items, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _log.WriteErrorLog("ExportCartDetail", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for a specific cart
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns> </returns>
        [HttpGet]
        [ApiKeyedRoute("cart/export/{cartId}")]
        public Models.OperationReturnModel<ExportOptionsModel> ExportCartDetail(Guid cartId)
        {
            Models.OperationReturnModel<ExportOptionsModel> retVal = new Models.OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.CartDetail, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportCartDetail", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Create a new cart from the quick add screen
        /// </summary>
        /// <param name="items">Items for the new cart</param>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("cart/quickadd")]
		public Models.OperationReturnModel<QuickAddReturnModel> QuickAddCart(List<QuickAddItemModel> items)
		{
            Models.OperationReturnModel<QuickAddReturnModel> retVal = new Models.OperationReturnModel<QuickAddReturnModel>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.CreateQuickAddCart(this.AuthenticatedUser, this.SelectedUserContext, items);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("POST QuickAddCart({0})", JsonConvert.SerializeObject(items)), ex);
                ExceptionEmail.Send(ex, string.Format("POST QuickAddCart({0})", JsonConvert.SerializeObject(items)));
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Validate if items are valid in the quick add screen
        /// </summary>
        /// <param name="items">Items to validate</param>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("cart/quickadd/validate")]
		public Models.OperationReturnModel<List<ItemValidationResultModel>> Validate(List<QuickAddItemModel> items)
		{
            Models.OperationReturnModel<List<ItemValidationResultModel>> retVal = new Models.OperationReturnModel<List<ItemValidationResultModel>>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.ValidateItems(this.SelectedUserContext, this.AuthenticatedUser, items);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("POST Validate", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Add a new item to an existing cart
        /// </summary>
        /// <param name="cartId">Cart id</param>
        /// <param name="newItem">Item</param>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public Models.OperationReturnModel<NewCSItem> AddItem(Guid cartId, ShoppingCartItem newItem)
		{
            Models.OperationReturnModel<NewCSItem> retVal = new Models.OperationReturnModel<NewCSItem>();
            try
            {
                retVal.SuccessResponse = new NewCSItem() { Id = _shoppingCartLogic.AddItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, newItem) };
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("AddItem({0})", JsonConvert.SerializeObject(newItem)), ex);
                ExceptionEmail.Send(ex, string.Format("AddItem({0})", JsonConvert.SerializeObject(newItem)));
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
		}

		/// <summary>
		/// Update item on an existing cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		/// <param name="updatedItem">Item</param>
		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public Models.OperationReturnModel<ShoppingCartItem> UpdateItem(Guid cartId, ShoppingCartItem updatedItem)
		{
            Models.OperationReturnModel<ShoppingCartItem> retVal = new Models.OperationReturnModel<ShoppingCartItem>();
            try
            {
                _shoppingCartLogic.UpdateItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, updatedItem);
                retVal.SuccessResponse = updatedItem;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("AddItem({0})", JsonConvert.SerializeObject(updatedItem)), ex);
                ExceptionEmail.Send(ex, string.Format("AddItem({0})", JsonConvert.SerializeObject(updatedItem)));
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Update existing cart
        /// </summary>
        /// <param name="updatedCart">Cart</param>
        /// <param name="deleteomitted">Delete items ommitted from the request?</param>
        [HttpPut]
		[ApiKeyedRoute("cart/")]
		public Models.OperationReturnModel<ShoppingCart> Put(ShoppingCart updatedCart, bool deleteomitted = true)
		{
            Models.OperationReturnModel<ShoppingCart> retVal = new Models.OperationReturnModel<ShoppingCart>();
            try
            {
                _shoppingCartLogic.UpdateCart(this.SelectedUserContext, this.AuthenticatedUser, updatedCart, deleteomitted);
                retVal.SuccessResponse = updatedCart;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("PUT Cart({0})", JsonConvert.SerializeObject(updatedCart)), ex);
                ExceptionEmail.Send(ex, string.Format("PUT Cart({0})", JsonConvert.SerializeObject(updatedCart)));
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Set a cart as the active cart. (Is this still used?)
        /// </summary>
        /// <param name="cartId"></param>
        [HttpPut]
		[ApiKeyedRoute("cart/{cartId}/active")]
		public Models.OperationReturnModel<Guid> SetActive(Guid cartId)
		{
            Models.OperationReturnModel<Guid> retVal = new Models.OperationReturnModel<Guid>();
            try
            {
                _activeCartLogic.SaveUserActiveCart(this.SelectedUserContext, this.AuthenticatedUser.UserId, cartId);
                retVal.SuccessResponse = cartId;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("PUT CartActive", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Delete a cart
        /// </summary>
        /// <param name="cartId">Cart id</param>
        [HttpDelete]
		[ApiKeyedRoute("cart/{cartId}")]
		public Models.OperationReturnModel<Guid> DeleteCart(Guid cartId)
		{
            Models.OperationReturnModel<Guid> retVal = new Models.OperationReturnModel<Guid>();
            try
            {
                _shoppingCartLogic.DeleteCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
                retVal.SuccessResponse = cartId;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DELETE Cart", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Delete a list of carts
        /// </summary>
        /// <param name="cartIds">List of cart ids</param>
        [HttpDelete]
		[ApiKeyedRoute("cart/")]
		public Models.OperationReturnModel<List<Guid>> DeleteCarts(List<Guid> cartIds)
		{
            Models.OperationReturnModel<List<Guid>> retVal = new Models.OperationReturnModel<List<Guid>>();
            try
            {
                _shoppingCartLogic.DeleteCarts(this.AuthenticatedUser, this.SelectedUserContext, cartIds);
                retVal.SuccessResponse = cartIds;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DELETE Carts", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Delete item from a cart
        /// </summary>
        /// <param name="cartId">Cart id</param>
        /// <param name="itemId">Item id</param>
        [HttpDelete]
		[ApiKeyedRoute("cart/{cartId}/item/{itemId}")]
		public Models.OperationReturnModel<bool> DeleteItem(Guid cartId, Guid itemId)
		{
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _shoppingCartLogic.DeleteItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, itemId);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DELETE Items", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }
        #endregion
    }
}
