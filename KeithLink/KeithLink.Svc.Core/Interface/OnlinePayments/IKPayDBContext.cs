using KeithLink.Svc.Core.Models.OnlinePayments;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.OnlinePayments.Log;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.OnlinePayments {
    public interface IKPayDBContext {

        //void OnModelCreating(DbModelBuilder modelBuilder);

        DbSet<AchRoleEmail> AchRoleEmails { get; set; }
        DbSet<ApplicationLog> ApplicationLogs { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<AuthenticationLog> AuthenticationLogs { get; set; }
        DbSet<KeithLink.Svc.Core.Models.OnlinePayments.Customer.Customer> Customers { get; set; }
        DbSet<CustomerBank> CustomerBanks { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Dsr> DSRs { get; set; }
        DbSet<Invoice> Invoices { get; set; }
        DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        DbSet<ProcessLog> ProcessLogs { get; set; }
        DbSet<State> States { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
        
    }
}
