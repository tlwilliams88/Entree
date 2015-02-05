using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Linq;
using KeithLink.Common.Core.Extensions;


namespace KeithLink.Svc.Impl.Migrations
{
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

            //context.Dsrs.AddOrUpdate(
            //    d => d.DsrNumber,
            //    new Dsr {
            //        DsrNumber = "066",
            //        Name = "Ochoa, Raul",
            //        BranchId = "FAM",
            //        Phone = "4328896994",
            //        EmailAddress = "riochoa@benekeith.com",
            //        ImageUrl = "/img/avatar.jpg"
            //    } );

            List<Dsr> defaultDsrs = new List<Dsr>();
            defaultDsrs.Add( new Dsr() { BranchId = "FAM", DsrNumber = "000", EmailAddress = "", Phone = "8006589790", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/fam@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FDF", DsrNumber = "000", EmailAddress = "", Phone = "8773186100", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/fdf@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FHS", DsrNumber = "000", EmailAddress = "", Phone = "8553275500", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/fhs@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FLR", DsrNumber = "000", EmailAddress = "", Phone = "8006882356", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/flr@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FAR", DsrNumber = "000", EmailAddress = "", Phone = "8006882356", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/far@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FAQ", DsrNumber = "000", EmailAddress = "", Phone = "8006752949", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/faq@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FOK", DsrNumber = "000", EmailAddress = "", Phone = "4057537911", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/fok@benekeith.com" } );
            defaultDsrs.Add( new Dsr() { BranchId = "FSA", DsrNumber = "000", EmailAddress = "", Phone = "2105076151", Name = "Ben E. Keith", ImageUrl = "{baseUrl}userimages/fsa@benekeith.com" } );

            context.Dsrs.AddOrUpdate(
                d => new { d.DsrNumber, d.BranchId },
                defaultDsrs.ToArray()
                );

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
					Subject = "Welcome to Entrée",
					IsBodyHtml = false,
					Type = MessageTemplateType.Email,
					Body = "Thank you for your interest in the Entrée System, Powered by Ben E. Keith.\r\n\r\n" + 
						   "If you have comments or questions, or would like someone to contact you, please e-mail us at {contactEmail}"
				}
				);

            /**** User account created temporary password email ****/
            System.Text.StringBuilder newUserPasswordBody = new System.Text.StringBuilder();
            newUserPasswordBody.AppendLine( "Welcome to Entrée!" );
            newUserPasswordBody.AppendLine();
            newUserPasswordBody.AppendLine( "An account has been created for you. Please use the temporary password to login and get started" );
            newUserPasswordBody.AppendLine();
            newUserPasswordBody.AppendLine( "Password: {password}" );
            newUserPasswordBody.AppendLine( "Url: {url}");
            newUserPasswordBody.AppendLine();

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "CreatedUserWelcome",
                    Subject = "Welcome to Entrée",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = newUserPasswordBody.ToString()
                } );

            /**** Forgot password email template ****/
            System.Text.StringBuilder resetPasswordBody = new System.Text.StringBuilder();
            resetPasswordBody.AppendLine( "Your Entrée password has changed" );
            resetPasswordBody.AppendLine(  );
            resetPasswordBody.AppendLine( "You recently changed your password for Ben E. Keith's Entrée system. If you feel this was done in error please contact support. " );
            resetPasswordBody.AppendLine(  );
            resetPasswordBody.AppendLine( "Temporary password: {password}" );
            resetPasswordBody.AppendLine( "Url: {url}" );
            resetPasswordBody.AppendLine();

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "ResetPassword",
                    Subject = "Your Ben E. Keith Entrée password has been changed",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = resetPasswordBody.ToString()
                } );

            /**** Build ETA notification main template ****/
            System.Text.StringBuilder etaNotificationBody = new System.Text.StringBuilder();
            etaNotificationBody.AppendLine("The following orders have been shipped or estimated for shipment.");
            etaNotificationBody.AppendLine();
            etaNotificationBody.AppendLine("{EtaOrderLines}");
            etaNotificationBody.AppendLine("All times shown in {TimeZoneName}.");
            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderEtaMain",
                    Subject = "Estimated Delivery Information for {CustomerName}",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = etaNotificationBody.ToString()
                });

            /**** Build ETA line template ****/
            System.Text.StringBuilder etaOrderLine = new System.Text.StringBuilder();
            etaOrderLine.Append("Order #: {InvoiceNumber} ");
            etaOrderLine.AppendLine(" is scheduled for delivery on {ScheduledDeliveryDate} at {ScheduledDeliveryTime}.");
            etaOrderLine.AppendLine("    It is currently estimated for delievery on {EstimatedDeliveryDate} at {EstimatedDeliveryTime}.");
            etaOrderLine.AppendLine("    This order contains {ProductCount} products (total quantity: {ShippedQuantity})");
            etaOrderLine.AppendLine();
            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderEtaLine",
                    Subject = "",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = etaOrderLine.ToString()
                });


            /**** Build Payment Confirmation main template ****/
            System.Text.StringBuilder paymentConfirmation = new System.Text.StringBuilder();
            paymentConfirmation.AppendLine("<p>Your payments have been scheduled for processing on the next business day.<p>");
            paymentConfirmation.AppendLine("Account: {CustomerNumber} - {CustomerName}<br/>");
            paymentConfirmation.AppendLine("Branch: {BranchId}<br/>");
            paymentConfirmation.AppendLine("Bank: {BankAccount}<br/>");
            paymentConfirmation.AppendLine("<table style=\"width: 100%\">");
            paymentConfirmation.AppendLine("	<tr>");
            paymentConfirmation.AppendLine("		<th>Ref</th>");
            paymentConfirmation.AppendLine("		<th>Number</th>");
            paymentConfirmation.AppendLine("		<th>Ref Date</th>");
            paymentConfirmation.AppendLine("		<th>Due Date</th>");
            paymentConfirmation.AppendLine("		<th>Scheduled</th>");
            paymentConfirmation.AppendLine("		<th>Amount</th>");
            paymentConfirmation.AppendLine("	</tr>");
            paymentConfirmation.AppendLine("{PaymentDetailLines}");
            paymentConfirmation.AppendLine("	<tr>");
            paymentConfirmation.AppendLine("		<td colspan=\"5\" style=\"text-align: right\">Customer Total</td>");
            paymentConfirmation.AppendLine("		<td style=\"text-align: right\">");
            paymentConfirmation.AppendLine("			{TotalPayments:f2}");
            paymentConfirmation.AppendLine("		<td>");
            paymentConfirmation.AppendLine("	</tr>");
            paymentConfirmation.AppendLine("</table>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "PaymentConfirmation",
                    Subject = "Payment Confirmation for {CustomerName}",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = paymentConfirmation.ToString()
                });

            /**** Build Payment Confirmation Detail template ****/
            System.Text.StringBuilder paymentConfirmationDetail = new System.Text.StringBuilder();
            paymentConfirmationDetail.AppendLine("	<tr>");
            paymentConfirmationDetail.AppendLine("		<td>{InvoiceType}</td>");
            paymentConfirmationDetail.AppendLine("		<td>{InvoiceNumber}</td>");
            paymentConfirmationDetail.AppendLine("		<td>{InvoiceDate:MM/dd/yyyy}</td>");
            paymentConfirmationDetail.AppendLine("		<td>{DueDate:MM/dd/yyyy}</td>");
            paymentConfirmationDetail.AppendLine("		<td>{ScheduledDate:MM/dd/yyyy}</td>");
            paymentConfirmationDetail.AppendLine("		<td style=\"text-align: right\">{PaymentAmount:f2}</td>");
            paymentConfirmationDetail.AppendLine("	</tr>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "PaymentConfirmationDetail",
                    Subject = "",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = paymentConfirmationDetail.ToString()
                });
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
