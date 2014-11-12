using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Log.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Profile.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
    public class KPayDBContext : DbContext, IKPayDBContext {
        #region ctor
        public KPayDBContext() {
            Database.SetInitializer<KPayDBContext>(null);
        }
        public KPayDBContext(string nameOrConnectionString) : base(nameOrConnectionString) {
            Database.SetInitializer<KPayDBContext>(null);
        }
        public KPayDBContext(DbConnection existingConnection) : base(existingConnection, true) {
            Database.SetInitializer<KPayDBContext>(null);
        }
        #endregion

        #region methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<AchRoleEmail>()
                .ToTable("AchRoleEmail");

            modelBuilder.Entity<ApplicationLog>()
                .ToTable("ApplicationLog")
                .MapToStoredProcedures(s => 
                    s.Insert(i => i.HasName("procInsertApplicationLog")
                                      .Parameter(p => p.UserName, "User")
                                      .Parameter(p => p.Message, "Msg"))
                            );

            modelBuilder.Entity<ApplicationUser>()
                .ToTable("ApplicationUser")
                .MapToStoredProcedures(s => {
                    s.Delete(p => p.HasName("procDeleteUser"));
                    s.Insert(i => i.HasName("procInsertUser")
                                   .Parameter(p => p.UserName, "UserName")
                                   .Parameter(p => p.FirstName, "FName")
                                   .Parameter(p => p.LastName, "LName")
                                   .Parameter(p => p.EmailAddress, "Email")
                                   .Parameter(p => p.Company, "Company")
                                   .Parameter(p => p.PhoneNumber, "Phone")
                                   .Parameter(p => p.HashedPassword, "Password")
                                   .Parameter(p => p.EmailConfirmation, "Confirm")
                                   .Parameter(p => p.Administrator, "Admin")
                                   .Parameter(p => p.AcceptedAgreement, "Agree")
                            );
                    s.Update(up => up.HasName("procUpdateUser")
                                   .Parameter(p => p.UserName, "UserName")
                                   .Parameter(p => p.FirstName, "FName")
                                   .Parameter(p => p.LastName, "LName")
                                   .Parameter(p => p.EmailAddress, "Email")
                                   .Parameter(p => p.Company, "Company")
                                   .Parameter(p => p.PhoneNumber, "Phone")
                                   .Parameter(p => p.HashedPassword, "Password")
                                   .Parameter(p => p.EmailConfirmation, "Confirm")
                                   .Parameter(p => p.Administrator, "Admin")
                                   .Parameter(p => p.AcceptedAgreement, "Agree")
                            );
                });

            modelBuilder.Entity<AuthenticationLog>()
                .ToTable("AuthenticationLog")
                .MapToStoredProcedures(s => {
                    s.Insert(i => i.HasName("procInsertAuthenticationLog")
                                   .Parameter(p => p.UserName, "User")
                                   .Parameter(p => p.Message, "Msg")
                            );
                });

            modelBuilder.Entity<Branch>()
                .ToTable("Division");

            modelBuilder.Entity<CustomerBank>()
                .ToTable("CustomerBank")
                .MapToStoredProcedures(s => {
                    s.Delete(d => d.HasName("procDeleteCustomerBankAccount")
                                   .Parameter(p => p.Division, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.AccountNumber, "AcctNum")
                            );
                    s.Insert(i => i.HasName("procInsertCustomerBankAccount")
                                   .Parameter(p => p.Division, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.AccountNumber, "AcctNum")
                                   .Parameter(p => p.TransitNumber, "Transit")
                                   .Parameter(p => p.Name, "Name")
                                   .Parameter(p => p.Address1, "Add1")
                                   .Parameter(p => p.Address2, "Add2")
                                   .Parameter(p => p.City, "City")
                                   .Parameter(p => p.State, "State")
                                   .Parameter(p => p.Zip, "Zip")
                                   .Parameter(p => p.DefaultAccount, "Default")
                            );
                });

            modelBuilder.Entity<Customer>()
                .ToTable("Customer");

            modelBuilder.Entity<Dsr>()
                .ToTable("Dsr");

            modelBuilder.Entity<Invoice>()
                .ToTable("Invoice")
                .MapToStoredProcedures(s => {
                    s.Delete(d => d.HasName("procDeleteInvoice")
                                   .Parameter(p => p.BranchId, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.InvoiceNumber, "Invoice")
                        );
                });

            modelBuilder.Entity<LastConfirmation>()
                .ToTable("LastConfirmation");
                
            modelBuilder.Entity<PaymentTransaction>()
                .ToTable("PaymentTransaction")
                .MapToStoredProcedures(s => {
                    s.Insert(i => i.HasName("procInsertTransaction")
                                   .Parameter(p => p.BranchId, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.InvoiceNumber, "Invoice")
                                   .Parameter(p => p.AccountNumber, "Account")
                                   .Parameter(p => p.UserName, "UserName")
                                   .Parameter(p => p.PaymentAmount, "Amount")
                                   .Parameter(p => p.ConfirmationId, "ConfId")
                        );
                });

            modelBuilder.Entity<ProcessLog>()
                .ToTable("ProcessLog");

            modelBuilder.Entity<State>()
                .ToTable("State");

            modelBuilder.Entity<UserGroup>()
                .ToTable("UserGroup")
                .MapToStoredProcedures(s => {
                    s.Delete(d => d.HasName("procDeleteCustomerInGroup")
                                   .Parameter(p => p.UserName, "Username")
                                   .Parameter(p => p.BranchId, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                        );
                    s.Insert(i => i.HasName("procInsertCustomerInGroup")
                                   .Parameter(p => p.UserName, "UserName")
                                   .Parameter(p => p.BranchId, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.InquireOnly, "Inquire")
                        );
                    s.Update(u => u.HasName("procUpdateCustomerInGroup")
                                   .Parameter(p => p.UserName, "UserName")
                                   .Parameter(p => p.BranchId, "Division")
                                   .Parameter(p => p.CustomerNumber, "CustNum")
                                   .Parameter(p => p.InquireOnly, "Inquire")
                        );
                });
        }
        #endregion

        #region properties
        public DbContext Context {
            get {
                return this;
            } 
        }
        public DbSet<AchRoleEmail> AchRoleEmails { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AuthenticationLog> AuthenticationLogs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerBank> CustomerBanks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Dsr> DSRs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<ProcessLog> ProcessLogs { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        #endregion
    }
}
