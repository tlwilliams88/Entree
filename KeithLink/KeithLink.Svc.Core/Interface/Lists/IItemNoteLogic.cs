using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
	public interface IItemNoteLogic
	{
		List<ItemNote> ReadNotes(Guid userId);
		ItemNote ReadNoteForItem(Guid userId, string itemNumber);

		void AddNote(Guid userId, ItemNote note);
		void DeleteNote(Guid userId, string itemNumber);

	}
}
