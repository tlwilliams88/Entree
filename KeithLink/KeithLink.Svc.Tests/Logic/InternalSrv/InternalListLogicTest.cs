using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Test.Mock;
using FizzWare.NBuilder;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using Autofac;
using KeithLink.Svc.Core.Models.Lists;
namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class InternalListLogicTests
    {
		private readonly IInternalListLogic listLogic;
		public InternalListLogicTests()
		{
			var container = DependencyMap.Build();

			listLogic = container.Resolve<IInternalListLogic>();				
		}

		[TestMethod]
		public void AddItem()
		{
			listLogic.AddItem(1, new Core.Models.Lists.ListItemModel() { ItemNumber = "546609", ParLevel = 5 });
		}

		[TestMethod]
		public void AddItems()
		{
			listLogic.AddItems(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1, new List<Core.Models.Lists.ListItemModel>() { new ListItemModel() { ItemNumber = "546609" } });
		}

		[TestMethod]
		public void AddNote()
		{
			listLogic.AddNote(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, new Core.Models.SiteCatalog.ItemNote() { ItemNumber = "546609", Note="test" });
		}
		
		[TestMethod]
		public void AddRecentlyViewedItem()
		{
			listLogic.AddRecentlyViewedItem(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, "546609");
		}

		[TestMethod]
		public void CreateList()
		{
			listLogic.CreateList(null, TestSessionObject.TestUserContext, new ListModel() { Name = "Test" }, Core.Enumerations.List.ListType.Custom);
		}
		
		[TestMethod]
		public void DeleteItem()
		{
			listLogic.DeleteItem(1);
		}
		
		[TestMethod]
		public void DeleteList()
		{
			listLogic.DeleteList(1);
		}

		[TestMethod]
		public void DeleteItemNumberFromList()
		{
			listLogic.DeleteItemNumberFromList(1, "023011");
		}
		
		[TestMethod]
		public void DeleteNote()
		{
			listLogic.DeleteNote(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, "023011");
		}

		[TestMethod]
		public void ReadFavories()
		{
			var favorites = listLogic.ReadFavorites(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext);
			Assert.IsNotNull(favorites);
		}
        
		[TestMethod]
		public void ReadList()
		{
			var list = listLogic.ReadList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1, false);

			Assert.AreEqual(list.ListId, 1);
		}

		[TestMethod]
		public void ReadListByType()
		{
			var list = listLogic.ReadListByType(TestSessionObject.TestUserContext, Core.Enumerations.List.ListType.Custom);
			Assert.IsNotNull(list);
		}
		
		[TestMethod]
		public void ReadListLabels()
		{
			var labels = listLogic.ReadListLabels(TestSessionObject.TestAuthenticatedUser,TestSessionObject.TestUserContext);
			Assert.IsNotNull(labels);
		}
		
		[TestMethod]
		public void ReadNotes()
		{
			var notes = listLogic.ReadNotes(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext);
			Assert.IsNotNull(notes);
		}

		[TestMethod]
		public void ReadRecent()
		{
			var recent = listLogic.ReadRecent(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext);
			Assert.IsNotNull(recent);
		}
		
		[TestMethod]
		public void ReadReminders()
		{
			var reminders = listLogic.ReadReminders(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext);
			Assert.IsNotNull(reminders);
		}
		
		[TestMethod]
		public void ReadUserList()
		{
			var list = listLogic.ReadUserList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext);
			Assert.IsNotNull(list);
		}
		
		[TestMethod]
		public void UpdateItem()
		{
			var list = listLogic.ReadList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1);
			listLogic.UpdateItem(list.Items[0]);
		}
		
		[TestMethod]
		public void UpdateList()
		{
			var list = listLogic.ReadList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1);
			listLogic.UpdateList(list);
		}

		[TestMethod]
		public void CopyList()
		{
			var restuls = listLogic.CopyList(new ListCopyShareModel() { ListId = 1, Customers = new List<Customer>{ new Customer { CustomerNumber = "410300",CustomerBranch = "FAM" } } });
		}
		
		[TestMethod]
		public void ShareList()
		{
			listLogic.ShareList(new ListCopyShareModel() { Customers = new List<Customer> { new Customer { CustomerNumber = "410300", CustomerBranch = "FAM" } } });
		}

		[TestMethod]
		public void ReadRecommendedItemsList()
		{
			var list = listLogic.ReadRecommendedItemsList(TestSessionObject.TestUserContext);
			Assert.IsNotNull(list);
		}
		
		[TestMethod]
		public void GetBarcodeForList()
		{
			var list = listLogic.GetBarcodeForList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1);
			Assert.IsNotNull(list);
		}
		
		[TestMethod]
		public void ReadPagedList()
		{
			var list = listLogic.ReadPagedList(TestSessionObject.TestAuthenticatedUser, TestSessionObject.TestUserContext, 1, new Core.Models.Paging.PagingModel() { Size = 2, From = 0 });
			Assert.AreEqual(list.Items.Results.Count, 2);
		}
		
		[TestMethod]
		public void ItemsInHistoryList()
		{
			var list = listLogic.ItemsInHistoryList(TestSessionObject.TestUserContext, new List<string>() { "023011" });
		}

        
    }
}
