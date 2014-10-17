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
	[Authorize]
    public class ItemNoteController : BaseController {
        #region attributes
        private readonly IItemNoteLogic itemNoteLogic;
        #endregion

        #region ctor
        public ItemNoteController(IItemNoteLogic itemNoteLogic,  IUserProfileLogic profileLogic) : base(profileLogic) {
			this.itemNoteLogic = itemNoteLogic;
        }
        #endregion

        #region methods
        [HttpPost]
		[ApiKeyedRoute("itemnote/")]
		public void AddItem(ItemNote newNote)
		{
			itemNoteLogic.AddNote(this.AuthenticatedUser, this.SelectedUserContext, newNote);
		}

		[HttpGet]
		[ApiKeyedRoute("itemnote/")]
		public List<ItemNote> Read()
		{
			return itemNoteLogic.ReadNotes(this.AuthenticatedUser, this.SelectedUserContext);
		}

		[HttpDelete]
		[ApiKeyedRoute("itemnote/{itemnumber}")]
		public void Delete(string itemNumber)
		{
			itemNoteLogic.DeleteNote(this.AuthenticatedUser, this.SelectedUserContext, itemNumber);
		}
        #endregion
    }
	
}
