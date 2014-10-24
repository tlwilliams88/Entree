using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
	public class BEKDBContext: DbContext
	{
		public BEKDBContext() { }
        public BEKDBContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
		public BEKDBContext(DbConnection existingConnection) : base(existingConnection, true) { }

		public DbSet<List> Lists { get; set; }
		public DbSet<ListItem> ListItems { get; set; }
		public DbSet<OrderHistoryDetail> OrderHistoryDetails { get; set; }
		public DbSet<OrderHistoryHeader> OrderHistoryHeaders { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<List>().ToTable("Lists", schemaName: "List").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<ListItem>().ToTable("ListItems", schemaName: "List").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<OrderHistoryHeader>().ToTable("OrderHistoryHeader", schemaName: "Orders").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<OrderHistoryDetail>().ToTable("OrderHistoryDetail", schemaName: "Orders").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);			
		}

		public override int SaveChanges()
		{
			var changeSet = ChangeTracker.Entries<BaseEFModel>();

			if (changeSet != null)
			{
				foreach (var entry in changeSet.Where(c => c.State != EntityState.Unchanged))
				{
					entry.Entity.ModifiedUtc = DateTime.UtcNow;
				}
			}
			return base.SaveChanges();
		}
	}
}
