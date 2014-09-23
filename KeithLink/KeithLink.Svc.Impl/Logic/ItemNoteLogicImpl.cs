using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT = KeithLink.Svc.Impl.RequestTemplates;
using KeithLink.Common.Core.Extensions;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Impl.Logic
{
	public class ItemNoteLogicImpl: IItemNoteLogic
	{
		private readonly IBasketRepository basketRepository;
		private const string NotesName = "Notes";

		public ItemNoteLogicImpl(IBasketRepository basketRepository)
		{
			this.basketRepository = basketRepository;
		}

		public List<ItemNote> ReadNotes(Guid userId)
		{
			var notes = RT.Orders.GetNotes(NotesName, userId.ToString("B"), "0", "false", "false");

			return ((CS.Basket)notes.First()).LineItems.Select(l => new ItemNote() { Note = l.Notes, ItemNumber = l.ProductId }).ToList();
		}

		public ItemNote ReadNoteForItem(Guid userId, string itemNumber)
		{
			throw new NotImplementedException();
		}

		public void AddNote(Guid userId, ItemNote note)
		{
			var basket = basketRepository.ReadBasket(userId, NotesName);

			if (basket.Status != NotesName)
			{
				var newBasket = new CS.Basket();
				newBasket.BranchId = string.Empty;
				newBasket.DisplayName = NotesName;
				newBasket.Status = NotesName;
				newBasket.Name = NotesName;

				basketRepository.CreateOrUpdateBasket(userId, string.Empty, newBasket, new List<CS.LineItem>() { new CS.LineItem() { ProductId = note.ItemNumber, Notes = note.Note, CatalogName = "fdf" } });
			}
			else
			{
				//Does item already exist? If so, just update the quantity
				var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(note.ItemNumber));
				if (existingItem.Any())
				{
					var updatedItem = existingItem.First();
					updatedItem.Notes = note.Note;
					basketRepository.UpdateItem(userId, basket.Id.ToGuid(), updatedItem);
				}
				else
					basketRepository.AddItem(userId, basket.Id.ToGuid(), new CS.LineItem() { Notes = note.Note, ProductId = note.ItemNumber}, basket);
			}
		}

		public void DeleteNote(Guid userId, string itemNumber)
		{
			var basket = basketRepository.ReadBasket(userId, NotesName);

			var item = basket.LineItems.Where(l => l.ProductId.Equals(itemNumber));

			if(item.Any())
				basketRepository.DeleteItem(userId, basket.Id.ToGuid(), item.First().Id.ToGuid());

		}
	}
}
