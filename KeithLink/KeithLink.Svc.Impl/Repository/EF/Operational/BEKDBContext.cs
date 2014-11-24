using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.ContentManagement.EF;
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
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<InvoiceItem> InvoiceItems { get; set; }
		public DbSet<BranchSupport> BranchSupports { get; set; }
		public DbSet<MessageTemplate> EmailTemplates { get; set; }
		public DbSet<Term> Terms { get; set; }
		public DbSet<ListShare> ListShares { get; set; }
        public DbSet<ContentItem> ContentItems { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<CustomerTopic> CustomerTopics { get; set; }
        public DbSet<UserMessagingPreference> UserMessagingPreferences { get; set; }
        public DbSet<UserPushNotificationDevice> UserPushNotificationDevices { get; set; }
        public DbSet<UserTopicSubscription> UserTopicSubscriptions { get; set; }
		public DbSet<ExportSetting> ExportSettings { get; set; }
        
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<List>().ToTable("Lists", schemaName: "List").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<ListItem>().ToTable("ListItems", schemaName: "List").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<OrderHistoryHeader>().ToTable("OrderHistoryHeader", schemaName: "Orders").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<OrderHistoryDetail>().ToTable("OrderHistoryDetail", schemaName: "Orders").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Invoice>().ToTable("Invoices", schemaName: "Invoice").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItems", schemaName: "Invoice").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<BranchSupport>().ToTable("BranchSupports", schemaName: "BranchSupport").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<UserMessage>().ToTable("UserMessages", schemaName: "Messaging").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<CustomerTopic>().ToTable("CustomerTopics", schemaName: "Messaging").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<MessageTemplate>().ToTable("MessageTemplates", schemaName: "Configuration").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<ExportSetting>().ToTable("ExportSettings", schemaName: "Configuration").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UserTopicSubscription>().ToTable("UserTopicSubscriptions", schemaName: "Messaging").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<UserMessagingPreference>().ToTable("UserMessagingPreferences", schemaName: "Messaging").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Term>().ToTable("Terms", schemaName: "Invoice").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ListShare>().ToTable("ListShares", schemaName: "List").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ContentItem>().ToTable("ContentItems", schemaName: "ContentManagement").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<UserPushNotificationDevice>().ToTable("UserPushNotificationDevices", schemaName: "Messaging").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

		public void DeleteTable(string tableName)
		{
			Database.ExecuteSqlCommand("DELETE " + tableName);
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
			try
			{
				return base.SaveChanges();
			}
			catch (System.Data.Entity.Validation.DbEntityValidationException e)
			{
				foreach (var eve in e.EntityValidationErrors)
				{
					Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
					{
						Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}
				throw;
			}
		}
	}
}
