using FizzWare.NBuilder;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Test.Mock
{
	public class UnitOfWorkMock : IUnitOfWork
	{
		#region Valid Values
		private List<string> PossibleItems = new List<string>() { "001195",
					"007001",
					"009083",
					"018075",
					"023005",
					"023011",
					"023025",
					"024015",
					"025010",
					"026005",
					"026055",
					"028074",
					"028197",
					"029013",
					"031110",
					"036020",
					"037008",
					"040090",
					"041100",
					"042030",
					"045700",
					"048190",
					"057011",
					"057015",
					"060013",
					"060193",
					"062075",
					"080097",
					"095026",
					"095047",
					"095135",
					"095220",
					"095480",
					"098022",
					"098029",
					"100658",
					"101732",
					"102025",
					"102754",
					"102967"
		};
		#endregion

		public BEKDBContext Context
		{
			get
			{
				#region Fake Db Sets
				var mockListSet = new FakeDbSet<List>();
				var mockListItemSet = new FakeDbSet<ListItem>();
				var mockShareSet = new FakeDbSet<ListShare>();
				var mockBranchSupportSet = new FakeDbSet<BranchSupport>();
				#endregion

				#region Generate data

				//List
				var listItemTemp = new List<ListItem>();
				IQueryable<List> listData;
				IQueryable<ListItem> listItemData;
				var shareItemData = new List<ListShare>().AsQueryable();
				IQueryable<BranchSupport> branchSupportData;

				GenerateListData(listItemTemp, out listData, out listItemData);

				branchSupportData = Builder<BranchSupport>.CreateListOfSize(10).Build().AsQueryable();
				
				#endregion

				mockListSet.SetData(listData);
				mockListItemSet.SetData(listItemData);
				mockShareSet.SetData(shareItemData);
				mockBranchSupportSet.SetData(branchSupportData);



				mockListSet.Setup(s => s.Include(It.IsAny<string>())).Returns(mockListSet.Object);

				#region Wire up Fake Db Sets with Mock Context
				var mockContext = new Mock<BEKDBContext>();
				mockContext.Setup(c => c.Set<List>()).Returns(mockListSet.Object);
				mockContext.Setup(c => c.Set<ListItem>()).Returns(mockListItemSet.Object);
				mockContext.Setup(c => c.Set<ListShare>()).Returns(mockShareSet.Object);
				mockContext.Setup(c => c.Set<BranchSupport>()).Returns(mockBranchSupportSet.Object);
				#endregion

				return mockContext.Object;
			}
		}

		private void GenerateListData(List<ListItem> listItemTemp, out IQueryable<List> listData, out IQueryable<ListItem> listItemData)
		{
			listData = Builder<List>.CreateListOfSize(20)
								.All()
									.With(x => x.CustomerId = "024418")
									.And(b => b.BranchId = "FDF")
									.And(x => x.Type = Core.Enumerations.List.ListType.Custom)
								.TheFirst(1)
									.With(x => x.Id = 1)
								.WhereRandom(1)
									.With(x => x.Type = Core.Enumerations.List.ListType.Notes)
								.WhereRandom(1)
									.With(x => x.Type = Core.Enumerations.List.ListType.Favorite)
								.Build()
								.AsQueryable();


			foreach (var list in listData)
			{
				list.Items = new List<ListItem>();
				IList<ListItem> items = null;
				if (list.Type == Core.Enumerations.List.ListType.Notes ||
					list.Type == Core.Enumerations.List.ListType.Favorite)
				{
					items = Builder<ListItem>.CreateListOfSize(5)
					.All()
					.Do(x => x.ItemNumber = PossibleItems[(int)x.Id])
					.Have(l => l.ParentList = list)
					.Build();
				}
				else
					items = Builder<ListItem>.CreateListOfSize(15)
					   .All()
					   .Do(x => x.ItemNumber = Pick<string>.RandomItemFrom(PossibleItems))
					   .Have(l => l.ParentList = list)
					   .Build();

				foreach (var item in items)
					list.Items.Add(item);

				listItemTemp.AddRange(items);
			}

			listItemData = listItemTemp.AsQueryable();
		}

		public IUnitOfWork GetUniqueUnitOfWork()
		{
			throw new NotImplementedException();
		}

		public int SaveChanges()
		{
			return this.Context.SaveChanges();
		}

		public int SaveChangesAndClearContext()
		{
			return SaveChanges();
		}

		public class FakeDbSet<T> : Mock<DbSet<T>> where T : class
		{
			HashSet<T> _data;
			IQueryable _query;

			public FakeDbSet()
			{
				_data = new HashSet<T>();
				_query = _data.AsQueryable();
			}

			public void Add(T item)
			{
				_data.Add(item);
			}

			public void Remove(T item)
			{
				_data.Remove(item);
			}

			public void Attach(T item)
			{
				_data.Add(item);
			}
			public void Detach(T item)
			{
				_data.Remove(item);
			}

			public T Create()
			{
				return Activator.CreateInstance<T>();
			}

			public ObservableCollection<T> Local
			{
				get
				{
					return new ObservableCollection<T>(_data);
				}
			}

			public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
			{
				return Activator.CreateInstance<TDerivedEntity>();
			}


			public void SetData(IQueryable<T> data)
			{
				this._query = data;
				this.As<IQueryable<T>>().Setup(x => x.Provider).Returns(data.Provider);
				this.As<IQueryable<T>>().Setup(x => x.Expression).Returns(data.Expression);
				this.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(data.ElementType);
				this.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
			}
		}
	}
}
