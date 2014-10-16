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
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Logic
{
	public class ItemNoteLogicImpl: IItemNoteLogic
	{
		private readonly IBasketRepository basketRepository;
		private readonly IUserProfileRepository userProfileRepository;

		private const string NotesName = "Notes";

		public ItemNoteLogicImpl(IBasketRepository basketRepository, IUserProfileRepository userProfileRepository)
		{
			this.basketRepository = basketRepository;
			this.userProfileRepository = userProfileRepository;
		}

		public List<ItemNote> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			//var notes = RT.Orders.GetNotes(NotesName, userId.ToCommerceServerFormat(), "0", "false", "false");

			var basket = RetrieveSharedList(user, catalogInfo);

			return basket.LineItems.Select(l => new ItemNote() { Note = l.Notes, ItemNumber = l.ProductId }).ToList();
		}

		public ItemNote ReadNoteForItem(Guid userId, string itemNumber)
		{
			throw new NotImplementedException();
		}

		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote note)
		{
			var basket = RetrieveSharedList(user, catalogInfo);

			if (basket.Status != NotesName)
			{
				var newBasket = new CS.Basket();
				newBasket.BranchId = string.Empty;
				newBasket.DisplayName = NotesName;
				newBasket.Status = NotesName;
				newBasket.Name = NotesName;

				basketRepository.CreateOrUpdateBasket(user.UserId, string.Empty, newBasket, new List<CS.LineItem>() { new CS.LineItem() { ProductId = note.ItemNumber, Notes = note.Note, CatalogName = catalogInfo.BranchId } });
			}
			else
			{
				//Does item already exist? If so, just update the quantity
				var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(note.ItemNumber));
				if (existingItem.Any())
				{
					var updatedItem = existingItem.First();
					updatedItem.Notes = note.Note;
					basketRepository.UpdateItem(user.UserId, basket.Id.ToGuid(), updatedItem);
				}
				else
					basketRepository.AddItem(basket.Id.ToGuid(), new CS.LineItem() { Notes = note.Note, ProductId = note.ItemNumber }, basket);
			}
		}

		private CS.Basket RetrieveSharedList(UserProfile user, UserSelectedContext catalogInfo)
		{
			var sharedUsers = userProfileRepository.GetUsersForCustomerOrAccount(user.UserCustomers.Where(c => c.CustomerNumber.Equals(catalogInfo.CustomerId)).First().CustomerId).Where(b => !b.UserId.Equals(user.UserId)).ToList();

			CS.Basket basket = null;

			foreach (var sharedUser in sharedUsers)
			{
				var sharedBasket = basketRepository.ReadBasket(sharedUser.UserId, NotesName);
				if (sharedBasket.Status == NotesName)
				{
					basket = sharedBasket;
					break;
				}
			}

			if (basket == null) 
				basket = basketRepository.ReadBasket(user.UserId, NotesName);
	
			return basket;
		}

		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			var basket = RetrieveSharedList(user, catalogInfo);

			var item = basket.LineItems.Where(l => l.ProductId.Equals(itemNumber));

			if(item.Any())
				basketRepository.DeleteItem(basket.UserId.ToGuid(), basket.Id.ToGuid(), item.First().Id.ToGuid());

		}
	}
}
