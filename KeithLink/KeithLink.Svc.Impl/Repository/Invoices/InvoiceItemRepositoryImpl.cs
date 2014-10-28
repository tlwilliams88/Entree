﻿using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Invoices
{
	public class InvoiceItemRepositoryImpl: EFBaseRepository<InvoiceItem>, IInvoiceItemRepository
	{
		public InvoiceItemRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) {
		
		}
	}
}
