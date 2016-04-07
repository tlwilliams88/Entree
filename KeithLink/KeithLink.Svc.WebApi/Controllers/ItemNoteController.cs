using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// item note controller
    /// </summary>
	[Authorize]
    public class ItemNoteController : BaseController {
        #region attributes
        private readonly IListLogic listServiceRepository;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="listServiceRepository"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        public ItemNoteController(IListLogic listServiceRepository,  IUserProfileLogic profileLogic, IEventLogRepository logRepo) : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Add item note
        /// </summary>
        /// <param name="newNote">Note</param>
        [HttpPost]
		[ApiKeyedRoute("itemnote/")]
		public Models.OperationReturnModel<bool> AddItem(ItemNote newNote)
		{
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                listServiceRepository.AddNote(this.AuthenticatedUser, this.SelectedUserContext, newNote);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("AddItem", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
		}

		/// <summary>
		/// Remove note from item
		/// </summary>
		/// <param name="itemNumber">Itemnumber</param>
		[HttpDelete]
		[ApiKeyedRoute("itemnote/{itemnumber}")]
		public Models.OperationReturnModel<bool> Delete(string itemNumber)
		{
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                listServiceRepository.DeleteNote(this.AuthenticatedUser, this.SelectedUserContext, itemNumber);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DeleteNote", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }
        #endregion
    }
	
}
