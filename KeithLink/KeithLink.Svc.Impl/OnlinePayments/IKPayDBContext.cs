using Entree.Core.Models.OnlinePayments.Customer.EF;
using Entree.Core.Models.OnlinePayments.Invoice.EF;
using Entree.Core.Models.OnlinePayments.Log.EF;
using Entree.Core.Models.OnlinePayments.Payment.EF;
using Entree.Core.Models.OnlinePayments.Profile.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.OnlinePayments {
    public interface IKPayDBContext {

        //void OnModelCreating(DbModelBuilder modelBuilder);

        DbContext Context { get; }
        DbSet<AchRoleEmail> AchRoleEmails { get; set; }
        DbSet<ApplicationLog> ApplicationLogs { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<AuthenticationLog> AuthenticationLogs { get; set; }
        DbSet<Entree.Core.Models.OnlinePayments.Customer.EF.Customer> Customers { get; set; }
        DbSet<CustomerBank> CustomerBanks { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Dsr> DSRs { get; set; }
        DbSet<Entree.Core.Models.OnlinePayments.Invoice.EF.Invoice> Invoices { get; set; }
        DbSet<Entree.Core.Models.OnlinePayments.Invoice.EF.InvoiceHeader> InvoiceHeaders { get; set; }
        DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        DbSet<ProcessLog> ProcessLogs { get; set; }
        DbSet<State> States { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
    }
}
