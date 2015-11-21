// KeithLink
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

// Mock Repo Libs
using FizzWare.NBuilder;
using Moq;

// Core
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	public class UnitOfWorkMock : IUnitOfWork {

        #region attributes

            private BEKDBContext _context;

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
        #endregion


		public BEKDBContext Context
		{
			get
			{
				if (_context == null)
				{

					#region Fake Db Sets

					var mockListSet = new FakeDbSet<List>();
					var mockListItemSet = new FakeDbSet<ListItem>();
					var mockShareSet = new FakeDbSet<ListShare>();
					var mockBranchSupportSet = new FakeDbSet<BranchSupport>();
                    var mockSettings = new FakeDbSet<Settings>();

					#endregion

					#region Generate data

					//List
					IQueryable<List> listData;
					IQueryable<ListItem> listItemData;
					var listItemTemp = new List<ListItem>();
                    var shareItemData = new List<ListShare>().AsQueryable();

                    GenerateListData( listItemTemp, out listData, out listItemData );

                    mockListSet.SetData( listData );
                    mockListItemSet.SetData( listItemData );
                    mockShareSet.SetData( shareItemData );
                    mockListSet.Setup(s => s.Include(It.IsAny<string>())).Returns(mockListSet.Object);

                    // Branch Support
                    IQueryable<BranchSupport> branchSupportData;
                    branchSupportData = Builder<BranchSupport>.CreateListOfSize( 10 ).Build().AsQueryable();
                    mockBranchSupportSet.SetData( branchSupportData );
                    //mockBranchSupportSet.Setup( x => x.Include( It.IsAny<string>() ) ).Returns( mockBranchSupportSet.Object );

                    // Settings
                    IQueryable<Settings> settingsData = Builder<Settings>.CreateListOfSize( 5 ).Build().AsQueryable();
                    mockSettings.SetData( settingsData );
                    
                    // Ensure any settings added to the repository are not null
                    mockSettings.Setup( x => x.Add( It.IsAny<Settings>() ) ).Callback((Settings s) => {
                        Assert.IsNotNull( s.UserId );
                        Assert.IsNotNull( s.Key );
                        Assert.IsNotNull( s.Value );

                        if (s.Id > 0) {
                            Assert.AreEqual( s.Value, "UpdatedValue" );
                        } else {
                            Assert.IsTrue( s.UserId == Guid.Parse("d616546e-463a-45ba-b1d4-d3512a56ace7") );
                            Assert.IsTrue( s.Key == "TestSetting" );
                            Assert.IsTrue( s.Value == "TestValue" );
                        }
                    } );

                    mockSettings.Setup( x => x.Remove( It.IsAny<Settings>() ) ).Callback( ( Settings s ) => {
                        Assert.IsNotNull( s.Id );
                    } );

                    #endregion


					#region Wire up Fake Db Sets with Mock Context

					var mockContext = new Mock<BEKDBContext>();
					mockContext.Setup(c => c.Set<List>()).Returns(mockListSet.Object);
					mockContext.Setup(c => c.Set<ListItem>()).Returns(mockListItemSet.Object);
					mockContext.Setup(c => c.Set<ListShare>()).Returns(mockShareSet.Object);
					mockContext.Setup(c => c.Set<BranchSupport>()).Returns(mockBranchSupportSet.Object);
                    mockContext.Setup( c => c.Set<Settings>() ).Returns( mockSettings.Object );

					#endregion


					_context = mockContext.Object;
					Database.SetInitializer<BEKDBContext>(null);
				
				}
				return _context;
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
