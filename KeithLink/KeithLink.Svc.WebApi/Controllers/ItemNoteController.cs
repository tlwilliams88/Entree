using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Impl.Service.List;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// item note controller
    /// </summary>
	[Authorize]
    public class ItemNoteController : BaseController {
        #region attributes
        private readonly IListLogic listServiceRepository;
        private readonly INotesListLogic _notesLogic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="listServiceRepository"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        public ItemNoteController(IListLogic listServiceRepository,  IUserProfileLogic profileLogic, INotesListLogic notesLogic,
            IEventLogRepository logRepo) : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
            _notesLogic = notesLogic;
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
                //_notesLogic.SaveNote(this.SelectedUserContext, newNote.ItemNumber, false, newNote.CatalogId, newNote.Note, true);
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
