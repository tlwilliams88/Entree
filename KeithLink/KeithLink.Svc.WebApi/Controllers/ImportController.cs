using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Import;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.WebApi.Helpers;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// ImportController
    /// </summary>
	[Authorize]
    public class ImportController : BaseController {
        #region attributes
        private readonly IImportLogic importLogic;
        private readonly IImportService _importService;
        private readonly IListService _listService;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="importLogic"></param>
        /// <param name="logRepo"></param>
        public ImportController(IUserProfileLogic profileLogic, IImportLogic importLogic, IEventLogRepository logRepo, IImportService importService, IListService listService)
			: base(profileLogic) {
            _listService = listService;
            _importService = importService;
			this.importLogic = importLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        #region Import List
        /// <summary>
		/// Import a user List
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("import/list")]
		public async Task<OperationReturnModel<ListImportModel>> List()
		{
            OperationReturnModel<ListImportModel> ret = new OperationReturnModel<ListImportModel>();
            try
            {
                var importReturn = new ListImportModel();

                ListImportFileModel fileModel = await ImportHelper.GetFileFromContent(Request.Content);

                ListModel newList = _importService.BuildList(this.AuthenticatedUser, this.SelectedUserContext, fileModel);

                importReturn.Success = true;
                importReturn.ListId = _listService.CreateList(AuthenticatedUser, SelectedUserContext, ListType.Custom, newList);

                _listService.SaveItems(AuthenticatedUser, SelectedUserContext, ListType.Custom, importReturn.ListId.Value, newList.Items);

                importReturn.WarningMessage = _importService.Warnings;
                importReturn.ErrorMessage = _importService.Errors;
                ret.SuccessResponse = importReturn;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _log.WriteErrorLog("Import List", ex);
            }
            return ret;
		}

        #endregion


        #region Import Order
        /// <summary>
        /// Import Order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("import/order")]
		public async Task<OperationReturnModel<OrderImportModel>> Order()
		{
            OperationReturnModel<OrderImportModel> ret = new OperationReturnModel<OrderImportModel>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new InvalidOperationException();

                OrderImportFileModel fileModel = new OrderImportFileModel();

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var content in provider.Contents)
                {
                    var file = content;
                    var paramName = file.Headers.ContentDisposition.Name.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    var stream = new MemoryStream(buffer);

                    using (var s = new StreamReader(stream))
                    {
                        switch (paramName)
                        {
                            case "file":
                                stream.CopyTo(fileModel.Stream);
                                fileModel.FileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                                stream.Seek(0, SeekOrigin.Begin); // Return to the start of the stream
                                fileModel.Contents = s.ReadToEnd();
                                break;
                            case "options":
                                fileModel.Options = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderImportOptions>(s.ReadToEnd());
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(fileModel.Contents) || fileModel.Options == null)
                {
                    throw new Exception("Invalid Request");
                }
                else
                {
                    ret.SuccessResponse = importLogic.ImportOrder(this.AuthenticatedUser, this.SelectedUserContext, fileModel);
                    ret.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _log.WriteErrorLog("Import Order", ex);
            }
            return ret;
        }
        #endregion

        #region Import Custom Inventory
        /// <summary>
		/// Import a user CustomInventory
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [ApiKeyedRoute("import/custominventory")]
        public async Task<OperationReturnModel<CustomInventoryImportModel>> CustomInventory()
        {
            OperationReturnModel<CustomInventoryImportModel> ret = new OperationReturnModel<CustomInventoryImportModel>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new InvalidOperationException();

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CustomInventoryImportFileModel fileModel = new CustomInventoryImportFileModel();

                foreach (var content in provider.Contents)
                {
                    var file = content;
                    var paramName = file.Headers.ContentDisposition.Name.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    var stream = new MemoryStream(buffer);

                    using (var s = new StreamReader(stream))
                    {
                        switch (paramName)
                        {
                            case "file":
                                stream.CopyTo(fileModel.Stream);
                                fileModel.FileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                                stream.Seek(0, SeekOrigin.Begin);
                                fileModel.Contents = s.ReadToEnd();
                                break;
                            case "options":
                                // Figure out what to do here
                                fileModel = Newtonsoft.Json.JsonConvert.
                                    DeserializeObject<CustomInventoryImportFileModel>(s.ReadToEnd());
                                break;
                        }
                    }
                }

                //if (string.IsNullOrEmpty(fileModel.Contents))
                //    return new ListImportModel() { Success = false, ErrorMessage = "Invalid request" };

                ret.SuccessResponse = importLogic.ImportCustomInventory
                    (this.AuthenticatedUser, this.SelectedUserContext, fileModel);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _log.WriteErrorLog("Import Custom Inventory", ex);
            }
            return ret;
        }
        #endregion
        #endregion
    }
}
