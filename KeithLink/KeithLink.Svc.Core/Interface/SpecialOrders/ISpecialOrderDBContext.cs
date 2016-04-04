using KeithLink.Svc.Core.Models.SpecialOrders.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.SpecialOrders {
    public interface ISpecialOrderDBContext {

        //void OnModelCreating(DbModelBuilder modelBuilder);


        DbContext Context { get; }
		DbSet<RequestHeaderId> RequestHeaderIds { get; set; }
        DbSet<RequestHeader> RequestHeaders { get; set; }
        DbSet<RequestItem> RequestItems { get; set; }
    }
}
