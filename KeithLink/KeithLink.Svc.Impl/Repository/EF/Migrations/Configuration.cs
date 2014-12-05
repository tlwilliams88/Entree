namespace KeithLink.Svc.Impl.Migrations
{
	using KeithLink.Svc.Core.Models.Configuration.EF;
	using KeithLink.Svc.Core.Models.EF;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Data.Entity.Migrations.Model;
	using System.Data.Entity.SqlServer;
	using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KeithLink.Svc.Impl.Repository.EF.Operational.BEKDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

			this.MigrationsDirectory = @"Repository\EF\Migrations";
			this.MigrationsNamespace = typeof(Configuration).Namespace;
			SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(KeithLink.Svc.Impl.Repository.EF.Operational.BEKDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

			context.BranchSupports.AddOrUpdate(
				b => b.BranchId,
				new BranchSupport
				{
					BranchName = "Amarillo",
					BranchId = "FAM",
					SupportPhoneNumber = "8064684555",
					TollFreeNumber = "8006589790x4555",
					Email = "Fam-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "Dallas/Fort Worth",
					BranchId = "FDF",
					SupportPhoneNumber = "8177596373",
					TollFreeNumber = "8773186100x6373",
					Email = "Fdf-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "Houston",
					BranchId = "FHS",
					SupportPhoneNumber = "8326525500",
					TollFreeNumber = "8553275500x5500",
					Email = "Fhs-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "Little Rock",
					BranchId = "FLR",
					SupportPhoneNumber = "5019785031",
					TollFreeNumber = "8006882356x5031",
					Email = "Flr-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "Little Rock",
					BranchId = "FAR",
					SupportPhoneNumber = "5019785031",
					TollFreeNumber = "8006882356x5031",
					Email = "Flr-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "New Mexico",
					BranchId = "FAQ",
					SupportPhoneNumber = "5056819950",
					TollFreeNumber = "8006752949x2308",
					Email = "Faq-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "Oklahoma",
					BranchId = "FOK",
					SupportPhoneNumber = "4057537911",
					TollFreeNumber = "",
					Email = "Fok-dis-mis@benekeith.com"
				},
				new BranchSupport
				{
					BranchName = "San Antonio",
					BranchId = "FSA",
					SupportPhoneNumber = "2105076151",
					TollFreeNumber = "After Hours: 2105076471",
					Email = "Fsa-dis-mis@benekeith.com"
				}
				);

			context.MessageTemplates.AddOrUpdate(
				t => t.TemplateKey,
				new MessageTemplate
				{
					TemplateKey = "GuestUserWelcome",
					Subject = "Welcome to Entr�e",
					IsBodyHtml = false,
					Type = MessageTemplateType.Email,
					Body = "Thank you for your interest in the Entr�e System, Powered by Ben E. Keith.\r\n\r\n" + 
						   "If you have comments or questions, or would like someone to contact you, please e-mail us at ${contactEmail}"
				}
				);
        }
    }
	internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
	{
		protected override void Generate(AddColumnOperation addColumnOperation)
		{
			SetCreatedUtcColumn(addColumnOperation.Column);

			base.Generate(addColumnOperation);
		}

		protected override void Generate(CreateTableOperation createTableOperation)
		{
			SetCreatedUtcColumn(createTableOperation.Columns);

			base.Generate(createTableOperation);
		}

		private static void SetCreatedUtcColumn(IEnumerable<ColumnModel> columns)
		{
			foreach (var columnModel in columns)
			{
				SetCreatedUtcColumn(columnModel);
			}
		}

		private static void SetCreatedUtcColumn(PropertyModel column)
		{
			if (column.Name == "CreatedUtc" || column.Name == "ModifiedUtc")
			{
				column.DefaultValueSql = "GETUTCDATE()";
			}
		}
	}
}
