﻿using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select InvoiceService.svc or InvoiceService.svc.cs at the Solution Explorer and start debugging.
	public class InvoiceService : IInvoiceService
	{
		private IInternalInvoiceLogic invoiceLogic;

		public InvoiceService(IInternalInvoiceLogic invoiceLogic)
		{
			this.invoiceLogic = invoiceLogic;
		}

		public List<InvoiceModel> ReadInvoices(UserSelectedContext catalogInfo)
		{
			return invoiceLogic.ReadInvoices(catalogInfo);
		}

		public InvoiceModel ReadInvoice(long Id)
		{
			return invoiceLogic.ReadInvoice(Id);
		}
	}
}
