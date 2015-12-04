using KeithLink.Svc.Core.Interface.SpecialOrders;
using KeithLink.Svc.Core.Models.SpecialOrders.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
    public class SpecialOrderDBContext : DbContext, ISpecialOrderDBContext {
        #region ctor
        public SpecialOrderDBContext() {
            Database.SetInitializer<SpecialOrderDBContext>(null);
        }
        public SpecialOrderDBContext(string nameOrConnectionString) : base(nameOrConnectionString) {
            Database.SetInitializer<SpecialOrderDBContext>(null);
        }
		public SpecialOrderDBContext(DbConnection existingConnection)
			: base(existingConnection, true)
		{
			Database.SetInitializer<SpecialOrderDBContext>(null);
        }
        #endregion

        #region methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<RequestHeader>()
                .ToTable("RequestHeader")
                .MapToStoredProcedures(s => {
                    s.Insert(i => i.HasName("spSaveRequestHeader")
								   .Parameter(p => p.RequestHeaderId, "RequestHeaderId")
								   .Parameter(p => p.BranchId, "BranchId")
								   .Parameter(p => p.CategoryId, "CategoryId")
								   .Parameter(p => p.BuyerNumber, "BuyerNumber")
								   .Parameter(p => p.DsrNumber, "DsrNumber")
								   .Parameter(p => p.Address, "Address")
								   .Parameter(p => p.City, "City")
								   .Parameter(p => p.State, "State")
								   .Parameter(p => p.Zip, "Zip")
								   .Parameter(p => p.Contact, "Contact")
								   .Parameter(p => p.ManufacturerName, "ManufacturerName")
								   .Parameter(p => p.ShipMethodId, "ShipMethodId")
								   .Parameter(p => p.OrderStatusId, "OrderStatusId")
								   .Parameter(p => p.SpecialInstructions, "SpecialInstructions")
								   .Parameter(p => p.ModifiedShippingAddress, "ModifiedShippingAddress")
								   .Parameter(p => p.CreditApproval, "CreditApproval")
								   .Parameter(p => p.StatusDate, "StatusDate")
								   .Parameter(p => p.SubmitDate, "SubmitDate")
								   .Parameter(p => p.UpdatedBy, "UpdatedBy")
                            );
                });
			
            modelBuilder.Entity<RequestItem>()
                .ToTable("RequestItem")
                .MapToStoredProcedures(s => {
                    s.Insert(i => i.HasName("spSaveRequestItem")
								   .Parameter(p => p.BranchId, "BranchId")
								   .Parameter(p => p.RequestHeaderId, "RequestHeaderId")
								   .Parameter(p => p.LineNumber, "LineNumber")
								   .Parameter(p => p.OrderStatusId, "OrderStatusId")
								   .Parameter(p => p.ManufacturerNumber, "ManufacturerNumber")
								   .Parameter(p => p.GtinUpc, "GtinUpc")
								   .Parameter(p => p.Description, "Description")
								   .Parameter(p => p.Quantity, "Quantity")
								   .Parameter(p => p.UnitOfMeasure, "UnitOfMeasure")
								   .Parameter(p => p.BekItemNumber, "BekItemNumber")
								   .Parameter(p => p.BekInvoiceNumber, "BekInvoiceNumber")
								   .Parameter(p => p.EstimateCost, "EstimateCost")
								   .Parameter(p => p.Price, "Price")
								   .Parameter(p => p.EsitamtedGPPercent, "EsitamtedGPPercent")
								   .Parameter(p => p.Comments, "Comments")
								   .Parameter(p => p.PONumber, "PONumber")
								   .Parameter(p => p.EstimatedArrival, "EstimatedArrival")
								   .Parameter(p => p.ArrivalDateFlag, "ArrivalDateFlag")
								   .Parameter(p => p.UpdatedBy, "UpdatedBy")
								   .Parameter(p => p.ShipMethodId, "ShipMethodId")
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
        public DbSet<RequestHeaderId> RequestHeaderIds { get; set; }
        public DbSet<RequestHeader> RequestHeaders { get; set; }
        public DbSet<RequestItem> RequestItems { get; set; }
        #endregion
    }
}
