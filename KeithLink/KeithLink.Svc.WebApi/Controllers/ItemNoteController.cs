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
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="listServiceRepository"></param>
        /// <param name="profileLogic"></param>
        public ItemNoteController(IListLogic listServiceRepository,  IUserProfileLogic profileLogic) : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
        }
        #endregion

        #region methods
		/// <summary>
		/// Add item note
		/// </summary>
		/// <param name="newNote">Note</param>
        [HttpPost]
		[ApiKeyedRoute("itemnote/")]
		public void AddItem(ItemNote newNote)
		{
			listServiceRepository.AddNote(this.AuthenticatedUser, this.SelectedUserContext, newNote);
		}

		/// <summary>
		/// Remove note from item
		/// </summary>
		/// <param name="itemNumber">Itemnumber</param>
		[HttpDelete]
		[ApiKeyedRoute("itemnote/{itemnumber}")]
		public void Delete(string itemNumber)
		{
			listServiceRepository.DeleteNote(this.AuthenticatedUser, this.SelectedUserContext, itemNumber);
		}
        #endregion
    }
	
}
