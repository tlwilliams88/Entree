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

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
    public class KPayDBContext : DbContext {
        #region ctor
        public KPayDBContext() { }
        public KPayDBContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public KPayDBContext(DbConnection existingConnection) : base(existingConnection, true) { }
        #endregion

        #region methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AchRoleEmail>().MapToStoredProcedures();
            modelBuilder.Entity<ApplicationLog>()
                .MapToStoredProcedures(proc => 
                    proc.Insert(i => i.HasName("procInsertApplicationLog")
                                      .Parameter(l => l.UserName, "User")
                                      .Parameter(l => l.Message, "Msg")));
        }
        #endregion

        #region properties
        #endregion
    }
}
