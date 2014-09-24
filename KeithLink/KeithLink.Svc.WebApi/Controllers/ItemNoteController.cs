﻿using KeithLink.Svc.Core.Interface.Lists;
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
    public class ItemNoteController : BaseController
    {
		private readonly IItemNoteLogic itemNoteLogic;

		public ItemNoteController(IItemNoteLogic itemNoteLogic,  IUserProfileRepository userProfileRepo)
            : base(userProfileRepo)
        {
			this.itemNoteLogic = itemNoteLogic;
        }

		[HttpPost]
		[ApiKeyedRoute("itemnote/")]
		public void AddItem(ItemNote newNote)
		{
			itemNoteLogic.AddNote(this.AuthenticatedUser.UserId, newNote);
		}

		[HttpGet]
		[ApiKeyedRoute("itemnote/")]
		public List<ItemNote> Read()
		{
			return itemNoteLogic.ReadNotes(this.AuthenticatedUser.UserId);
		}

		[HttpDelete]
		[ApiKeyedRoute("itemnote/{itemnumber}")]
		public void Delete(string itemNumber)
		{
			itemNoteLogic.DeleteNote(this.AuthenticatedUser.UserId, itemNumber);
		}
    }
	
}
