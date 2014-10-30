﻿using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Invoices
{
	public class InvoiceServiceRepositoryImpl: IInvoiceServiceRepository
	{		
		private com.benekeith.InvoiceService.IInvoiceService serviceClient;

		public InvoiceServiceRepositoryImpl(com.benekeith.InvoiceService.IInvoiceService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public List<InvoiceModel> ReadInvoices(UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadInvoices(catalogInfo).ToList();
		}

		public InvoiceModel ReadInvoice(long Id)
		{
			return serviceClient.ReadInvoice(Id);
		}
	}
}