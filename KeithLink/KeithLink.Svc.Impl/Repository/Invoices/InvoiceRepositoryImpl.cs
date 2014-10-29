using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.EF;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Invoices
{
	public class InvoiceRepositoryImpl : EFBaseRepository<Invoice>, IInvoiceRepository
	{
		public InvoiceRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        

	}
}
