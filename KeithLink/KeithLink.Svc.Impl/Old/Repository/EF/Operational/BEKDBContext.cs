﻿using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Models.EF;
using Entree.Core.Models.Configuration.EF;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Models.Orders.EF;
using Entree.Core.Models.Orders.History.EF;
using Entree.Core.Models.Profile.EF;
using Entree.Core.Models.Customers.EF;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
    public class BEKDBContext : DbContext {
        private IEventLogRepository _log;

        public BEKDBContext(IEventLogRepository logRepository) {
            _log = logRepository;
        }

		public BEKDBContext() {}

        public BEKDBContext( string nameOrConnectionString ) : base( nameOrConnectionString ) { }
        public BEKDBContext( DbConnection existingConnection ) : base( existingConnection, true ) { }

        // BranchSupport
        public DbSet<BranchSupport> BranchSupports { get; set; }
        public DbSet<Dsr> Dsrs { get; set; }
        public DbSet<DsrAlias> DsrAliases { get; set; }

        // Configuration
        public DbSet<ExportSetting> ExportSettings { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<ExternalCatalog> ExternalCatalogs { get; set; }

        // Customers
        public DbSet<ItemHistory> ItemHistory { get; set; }
        public DbSet<InternalUserAccess> InternalUserAccess { get; set; }

        // Invoices
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Term> Terms { get; set; }

        // Lists
        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<ListShare> ListShares { get; set; }
        public DbSet<CustomInventoryItem> CustomInventoryItems { get; set; }

        // Messaging
        public DbSet<CustomerTopic> CustomerTopics { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<UserMessagingPreference> UserMessagingPreferences { get; set; }
        public DbSet<UserPushNotificationDevice> UserPushNotificationDevices { get; set; }
        public DbSet<UserTopicSubscription> UserTopicSubscriptions { get; set; }

        // Orders
        public DbSet<OrderHistoryDetail> OrderHistoryDetails { get; set; }
        public DbSet<OrderHistoryHeader> OrderHistoryHeaders { get; set; }
        public DbSet<UserActiveCart> UserActiveCarts { get; set; }

        // Profile
		public DbSet<MarketingPreference> MarketingPreferences {get;set;}
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating( DbModelBuilder modelBuilder ) {
            // Lists
            modelBuilder.Entity<List>().ToTable( "Lists", schemaName: "List" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<ListItem>().ToTable( "ListItems", schemaName: "List" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<ListShare>().ToTable( "ListShares", schemaName: "List" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<CustomInventoryItem>().ToTable( "CustomInventoryItems", schemaName: "List" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );

            // Orders
            modelBuilder.Entity<OrderHistoryHeader>().ToTable( "OrderHistoryHeader", schemaName: "Orders" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<OrderHistoryDetail>().ToTable( "OrderHistoryDetail", schemaName: "Orders" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<UserActiveCart>().ToTable( "UserActiveCarts", schemaName: "Orders" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );

            // Invoices
            modelBuilder.Entity<Invoice>().ToTable( "Invoices", schemaName: "Invoice" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<InvoiceItem>().ToTable( "InvoiceItems", schemaName: "Invoice" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<Term>().ToTable( "Terms", schemaName: "Invoice" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );

            // BranchSupport
            modelBuilder.Entity<BranchSupport>().ToTable( "BranchSupports", schemaName: "BranchSupport" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<Dsr>().ToTable( "Dsrs", schemaName: "BranchSupport" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<DsrAlias>().ToTable( "DsrAliases", schemaName: "BranchSupport" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );

            // Messaging
            modelBuilder.Entity<UserMessage>().ToTable( "UserMessages", schemaName: "Messaging" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<CustomerTopic>().ToTable( "CustomerTopics", schemaName: "Messaging" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<UserTopicSubscription>().ToTable( "UserTopicSubscriptions", schemaName: "Messaging" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<UserMessagingPreference>().ToTable( "UserMessagingPreferences", schemaName: "Messaging" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<UserPushNotificationDevice>().ToTable( "UserPushNotificationDevices", schemaName: "Messaging" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );

            // Configuration
            modelBuilder.Entity<MessageTemplate>().ToTable( "MessageTemplates", schemaName: "Configuration" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<ExportSetting>().ToTable( "ExportSettings", schemaName: "Configuration" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
            modelBuilder.Entity<ExternalCatalog>().ToTable("ExternalCatalogs", schemaName: "Configuration").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Profile
            modelBuilder.Entity<MarketingPreference>().ToTable("MarketingPreferences", schemaName: "Profile").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Settings>().ToTable( "Settings", schemaName: "Profile" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
			modelBuilder.Entity<PasswordResetRequest>().ToTable("PasswordResetRequests", schemaName: "Profile").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Customers
            modelBuilder.Entity<ItemHistory>().ToTable("ItemHistory", schemaName: "Customers").Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<InternalUserAccess>().ToTable( "InternalUserAccess", schemaName: "Customers" ).Property( o => o.Id ).HasDatabaseGeneratedOption( DatabaseGeneratedOption.Identity );
        }

        public override int SaveChanges() {
            var changeSet = ChangeTracker.Entries<BaseEFModel>();

            if (changeSet != null) {
                foreach (var entry in changeSet.Where( c => c.State != EntityState.Unchanged )) {
                    entry.Entity.ModifiedUtc = DateTime.UtcNow;
                }
            }
            try {
                return base.SaveChanges();
            } catch (System.Data.Entity.Validation.DbEntityValidationException e) {
                // Create a custom error message with validation information
                StringBuilder errorDetails = new StringBuilder();

                // First message shoudl be the thrown error
                errorDetails.AppendLine( e.Message );

                // Loop through the validation errors and append them to the message
                foreach (var eve in e.EntityValidationErrors) {
                    errorDetails.AppendLine( String.Format( "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType(), eve.Entry.State ) );
                    foreach (var ve in eve.ValidationErrors) {
                        errorDetails.AppendLine( String.Format( " - Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage ) );
                    }
                }

				if(_log != null)
					_log.WriteErrorLog( errorDetails.ToString() );

                // Throw a new exception using the error details we have established
				throw new System.Data.Entity.Validation.DbEntityValidationException(errorDetails.ToString(), e);
            }
        }

        public void UndoDBContextChanges() {
            foreach (DbEntityEntry entry in this.ChangeTracker.Entries()) {
                if (entry.Entity != null) {
                    entry.State = EntityState.Detached;
                }
            }
        }
    }
}
