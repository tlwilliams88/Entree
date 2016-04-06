﻿using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Invoices
{
	public interface IInternalInvoiceLogic
	{
		long CreateInvoice(InvoiceModel invoice, InvoiceType type);
		List<InvoiceModel> ReadInvoices(UserProfile user, UserSelectedContext catalogInfo);
		InvoiceModel ReadInvoice(UserProfile user, UserSelectedContext catalogInfo, long Id);
		void DeleteInvoice(long Id);
		void UpdateInvoice(InvoiceModel invoice);
		TermModel ReadTermInformation(string branchId, string termCode);
	}
}