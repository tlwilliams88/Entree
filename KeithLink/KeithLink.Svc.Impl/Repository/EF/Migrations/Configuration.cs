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

            List<ExternalCatalog> extCatalogs = new List<ExternalCatalog>();
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "COR", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FAM", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FAQ", ExternalBranchId = "unfi_5", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FAR", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FDF", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FHS", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FLR", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FOK", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });
            extCatalogs.Add(new ExternalCatalog() { BekBranchId = "FSA", ExternalBranchId = "unfi_7", Type = ExternalCatalogType.UNFI });

            context.ExternalCatalogs.AddOrUpdate(c => new { c.BekBranchId, c.ExternalBranchId }, extCatalogs.ToArray());

			context.MessageTemplates.AddOrUpdate(
				t => t.TemplateKey,
				new MessageTemplate
				{
					TemplateKey = "GuestUserWelcome",
					Subject = "Welcome to Entr�e",
					IsBodyHtml = false,
					Type = MessageTemplateType.Email,
					Body = "Thank you for your interest in the Entr�e System, Powered by Ben E. Keith.\r\n\r\n" + 
						   "If you have comments or questions, or would like someone to contact you, please e-mail us at {contactEmail}"
				}
				);

            /**** User account created temporary password email ****/
            System.Text.StringBuilder newUserPasswordBody = new System.Text.StringBuilder();
			newUserPasswordBody.AppendLine("<div>");
			newUserPasswordBody.AppendLine("<p>");
			newUserPasswordBody.AppendLine("Welcome to Entr�e!");
			newUserPasswordBody.AppendLine("</p>");
			newUserPasswordBody.AppendLine("");
			newUserPasswordBody.AppendLine("<p>");
			newUserPasswordBody.AppendLine("An account has been created for you. Please use the following link to create a password for your account.");
			newUserPasswordBody.AppendLine("</p>");
			newUserPasswordBody.AppendLine("<p>");
			newUserPasswordBody.AppendLine("<a href=\"{resetLink}\" target=\"_blank\">{resetLink}</a>");
			newUserPasswordBody.AppendLine("</p>");
			newUserPasswordBody.AppendLine("<p>");
			newUserPasswordBody.AppendLine("If clicking the link doesn't seem to work, you can copy and paste the link into your browser's address window, or retype it there.");
			newUserPasswordBody.AppendLine("</p>");
			newUserPasswordBody.AppendLine("</div>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "CreatedUserWelcome",
                    Subject = "Welcome to Entr�e",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = newUserPasswordBody.ToString()
                } );

            /**** Forgot password email template ****/
            System.Text.StringBuilder resetPasswordBody = new System.Text.StringBuilder();
            resetPasswordBody.AppendLine( "Your Entr�e password has changed" );
            resetPasswordBody.AppendLine(  );
            resetPasswordBody.AppendLine( "You recently changed your password for Ben E. Keith's Entr�e system. If you feel this was done in error please contact support. " );
            resetPasswordBody.AppendLine(  );
            resetPasswordBody.AppendLine( "Temporary password: {password}" );
            resetPasswordBody.AppendLine( "Url: {url}" );
            resetPasswordBody.AppendLine();

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "ResetPassword",
                    Subject = "Your Ben E. Keith Entr�e password has been changed",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = resetPasswordBody.ToString()
                } );

            /**** Standard BEK header notification template ****/
            System.Text.StringBuilder headerNotificationBody = new System.Text.StringBuilder();
            headerNotificationBody.AppendLine("<table style=\"width: 100%;\">");
            headerNotificationBody.AppendLine("<tr>");
            headerNotificationBody.AppendLine("<td>|LOGO|</td>");
            headerNotificationBody.AppendLine("<td><h3>{Subject}</h3></td>");
            headerNotificationBody.AppendLine("<td style=\"text-align:right;\">");
            headerNotificationBody.AppendLine("<table>");
            headerNotificationBody.AppendLine("<tr>");
            headerNotificationBody.AppendLine("<td>{CustomerName}</td>");
            headerNotificationBody.AppendLine("</tr>");
            headerNotificationBody.AppendLine("<tr>");
            headerNotificationBody.AppendLine("<td>Customer: {CustomerNumber}</td>");
            headerNotificationBody.AppendLine("</tr>");
            headerNotificationBody.AppendLine("<tr>");
            headerNotificationBody.AppendLine("<td>Branch: {BranchID}</td>");
            headerNotificationBody.AppendLine("</tr>");
            headerNotificationBody.AppendLine("</table>");
            headerNotificationBody.AppendLine("</td>");
            headerNotificationBody.AppendLine("</tr>");
            headerNotificationBody.AppendLine("</table>");
            headerNotificationBody.AppendLine("<hr/>");
            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "NotifHeader",
                    Subject = "",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = headerNotificationBody.ToString()
                });

            /**** Build ETA notification main template ****/
            System.Text.StringBuilder etaNotificationBody = new System.Text.StringBuilder();
            etaNotificationBody.AppendLine("{NotifHeader}Order #: {InvoiceNumber} {ETAMessage}.");
            etaNotificationBody.AppendLine("This order contains {ProductCount} items.<hr/>");
            etaNotificationBody.AppendLine("<table style=\"width: 100%\">");
            etaNotificationBody.AppendLine("<tr>");
            etaNotificationBody.AppendLine("<th style=\"font-size:small;text-align:left;\">Item # </th>");
            etaNotificationBody.AppendLine("<th style=\"font-size:small;text-align:left;\">Item Description </th>");
            etaNotificationBody.AppendLine("<th colspan=\"2\" style=\"font-size:small;text-align:left;\">Quantity</th>");
            etaNotificationBody.AppendLine("</tr>");
            etaNotificationBody.AppendLine("{OrderEtaLines}");
            etaNotificationBody.AppendLine("</table>");
            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderEtaMain",
                    Subject = "Estimated Delivery Information for {CustomerNumber}-{CustomerName}",
                    IsBodyHtml = false,
                    Type = MessageTemplateType.Email,
                    Body = etaNotificationBody.ToString()
                });

            /**** Build ETA line template ****/
            System.Text.StringBuilder etaOrderLine = new System.Text.StringBuilder();
            etaOrderLine.Append("<tr>");
            etaOrderLine.Append("<td style=\"font-size:small;text-align:left;\">{ProductNumber} </td>");
            etaOrderLine.Append("<td style=\"font-size:small;text-align:left;\">{ProductDescription} </td>");
            etaOrderLine.Append("<td colspan=\"2\" style=\"font-size:small;text-align:left;\">{Quantity}</td>");
            etaOrderLine.Append("</tr>");
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
            paymentConfirmation.AppendLine("{NotifHeader}");
            paymentConfirmation.AppendLine("Bank: {BankAccount}<br/>");
            paymentConfirmation.AppendLine("Confirmation: {ConfirmationId}<br/>");
            paymentConfirmation.AppendLine("<table style=\"width: 100%\">");
            paymentConfirmation.AppendLine("<tr>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:left;\">Type </th>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:left;\">Number </th>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:left;\">Ref. Date </th>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:left;\">Due Date </th>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:left;\">Scheduled Date </th>");
            paymentConfirmation.AppendLine("<th style=\"font-size:small;text-align:right;\">Amount</th>");
            paymentConfirmation.AppendLine("</tr>");
            paymentConfirmation.AppendLine("{PaymentDetailLines}");
            paymentConfirmation.AppendLine("<tr>");
            paymentConfirmation.AppendLine("<td colspan=\"5\" style=\"text-align:right\">Customer Total</td>");
            paymentConfirmation.AppendLine("<td style=\"text-align:right\">");
            paymentConfirmation.AppendLine("${TotalPayments:f2}");
            paymentConfirmation.AppendLine("<td>");
            paymentConfirmation.AppendLine("</tr>");
            paymentConfirmation.AppendLine("</table>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "PaymentConfirmation",
                    Subject = "Payment Confirmation for {CustomerNumber}-{CustomerName}",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = paymentConfirmation.ToString()
                });

            /**** Build Payment Confirmation Detail template ****/
            System.Text.StringBuilder paymentConfirmationDetail = new System.Text.StringBuilder();
            paymentConfirmationDetail.AppendLine("<tr>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:left;\">{InvoiceType} </td>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:left;\">{InvoiceNumber} </td>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:left;\">{InvoiceDate:MM/dd/yyyy} </td>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:left;\">{DueDate:MM/dd/yyyy} </td>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:left;\">{ScheduledDate:MM/dd/yyyy} </td>");
            paymentConfirmationDetail.AppendLine("<td style=\"font-size:small;text-align:right;\">${PaymentAmount:f2}</td>");
            paymentConfirmationDetail.AppendLine("</tr>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate {
                    TemplateKey = "PaymentConfirmationDetail",
                    Subject = "",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = paymentConfirmationDetail.ToString()
                });


			System.Text.StringBuilder resetPasswordMessage = new System.Text.StringBuilder();
			resetPasswordMessage.AppendLine("<div>");
			resetPasswordMessage.AppendLine("	<p>");
			resetPasswordMessage.AppendLine("        We received a request to reset the password associated with this e-mail address. If you made this request, please follow the instructions below.");
			resetPasswordMessage.AppendLine("    </p>");
			resetPasswordMessage.AppendLine("	<p>");
			resetPasswordMessage.AppendLine("		Click the link below to reset your password:");
			resetPasswordMessage.AppendLine("	</p>");
			resetPasswordMessage.AppendLine("	<p>");
			resetPasswordMessage.AppendLine("		<a href=\"{resetLink}\" target=\"_blank\">{resetLink}</a>");
			resetPasswordMessage.AppendLine("	</p>");
			resetPasswordMessage.AppendLine("  <p>");
			resetPasswordMessage.AppendLine("    If you did not request to have your password reset you can safely ignore this email.");
			resetPasswordMessage.AppendLine("  </p>");
			resetPasswordMessage.AppendLine("  <p>");
			resetPasswordMessage.AppendLine("    If clicking the link doesn't seem to work, you can copy and paste the link into your browser's address window, or retype it there.");
			resetPasswordMessage.AppendLine("  </p>");
			resetPasswordMessage.AppendLine("  <p>");
			resetPasswordMessage.AppendLine("	The link will expire in 3 days, so be sure to use it right away");
			resetPasswordMessage.AppendLine("  </p>");
			resetPasswordMessage.AppendLine("</div>");

			context.MessageTemplates.AddOrUpdate(
				t => t.TemplateKey,
				new MessageTemplate
				{
					TemplateKey = "ResetPasswordRequest",
					Subject = "Ben E. Keith Entr�e Password Reset",
					IsBodyHtml = true,
					Type = MessageTemplateType.Email,
					Body = resetPasswordMessage.ToString()

				});


            System.Text.StringBuilder orderConfirmationMessage = new System.Text.StringBuilder();
            orderConfirmationMessage.AppendLine("{NotifHeader}");
            orderConfirmationMessage.AppendLine("<table style=\"width: 100%;\">");
            orderConfirmationMessage.AppendLine("   <tr>");
            orderConfirmationMessage.AppendLine("       <td>Delivery Date: {ShipDate}</td>");
            orderConfirmationMessage.AppendLine("       <td style='text-align:right;'>{Count} Items / {PcsCount} Pieces</td>");
            orderConfirmationMessage.AppendLine("   </tr>");
            orderConfirmationMessage.AppendLine("   <tr>");
            orderConfirmationMessage.AppendLine("       <td>Invoice Number: {InvoiceNumber}</td>");
            orderConfirmationMessage.AppendLine("       <td style=\"text-align:right;\">Order subtotal: ${Total}</td>");
            orderConfirmationMessage.AppendLine("   </tr>");
            orderConfirmationMessage.AppendLine("</table>");
            orderConfirmationMessage.AppendLine("<table style=\"width: 100%;\">");
            orderConfirmationMessage.AppendLine("	<tr style=\"border-bottom:1px solid gray;\">");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Item # </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Confirmed Items </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Brand </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Ordered </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Confirmed </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Pack </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Size </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Price </th>");
            orderConfirmationMessage.AppendLine("		<th style=\"text-align:left;\">Status</th>");
            orderConfirmationMessage.AppendLine("	</tr>");
            orderConfirmationMessage.AppendLine("{OrderConfirmationItems}");
            orderConfirmationMessage.AppendLine("</table>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderConfirmation",
                    Subject = "Ben E. Keith: {OrderStatus} for {CustomerNumber}-{CustomerName};{InvoiceNumber}",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = orderConfirmationMessage.ToString()
                });

            System.Text.StringBuilder orderConfirmationItemDetailMessage = new System.Text.StringBuilder();
            orderConfirmationItemDetailMessage.AppendLine("<tr>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{ProductNumber} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{ProductDescription} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Brand} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Quantity} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Sent} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Pack} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Size} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Price} </td>");
            orderConfirmationItemDetailMessage.AppendLine("<td style=\"font-size:small;text-align:left;\">{Status}</td>");
            orderConfirmationItemDetailMessage.AppendLine("</tr>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderConfirmationItemDetail",
                    Subject = "",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = orderConfirmationItemDetailMessage.ToString()
                });

            System.Text.StringBuilder orderConfirmationItemOOSDetailMessage = new System.Text.StringBuilder();
            orderConfirmationItemOOSDetailMessage.AppendLine("<tr>");
            orderConfirmationItemOOSDetailMessage.AppendLine("      <td style=\"text-align:left;color:maroon;\">{ProductNumber} </td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("		<td style=\"text-align:left;color:maroon;\">{ProductDescription} </td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("		<td style=\"text-align:left;color:maroon;\">{Quantity} </td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("		<td style=\"text-align:left;color:maroon;\">{Sent} </td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("		<td style=\"text-align:left;color:maroon;\">{Price} </td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("		<td style=\"text-align:left;color:maroon;\">{Status}</td>");
            orderConfirmationItemOOSDetailMessage.AppendLine("</tr>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderConfirmationItemOOSDetail",
                    Subject = "",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = orderConfirmationItemOOSDetailMessage.ToString()
                });

            System.Text.StringBuilder orderRejectedMessage = new System.Text.StringBuilder();
            orderRejectedMessage.AppendLine("Order Rejected: {SpecialInstructions}<br/>");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "OrderRejected",
                    Subject = "<h3>{CustomerName}</h3><h3>Customer # {CustomerNumber}</h3><h3 style=\"color:maroon;\">{SpecialInstructions}</h3>",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = orderRejectedMessage.ToString()
                });

            System.Text.StringBuilder dsrRequestMessage = new System.Text.StringBuilder();
            dsrRequestMessage.AppendLine("{NotifHeader}{UserFirstName} {UserLastName} would like to be contacted regarding the following item: {ItemNumber}.");
            dsrRequestMessage.AppendLine(" This user can be contacted by e-mail at {UserEmail}");

            context.MessageTemplates.AddOrUpdate(
                t => t.TemplateKey,
                new MessageTemplate
                {
                    TemplateKey = "DSRContactRequest",
                    Subject = "Ben E. Keith: DSR Contact Request for {CustomerNumber}-{CustomerName}",
                    IsBodyHtml = true,
                    Type = MessageTemplateType.Email,
                    Body = dsrRequestMessage.ToString()
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
