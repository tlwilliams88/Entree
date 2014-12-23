using EF = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Invoice {
    public interface IKPayInvoiceRepository {
        void DeleteInvoice(string division, string customerNumber, string invoiceNumber);

        //List<EF.Invoice> GetAllOpenInvoices(string division, string customerNumber);

        //List<EF.Invoice> GetAllPaidInvoices(string division, string customerNumber);

        //List<EF.Invoice> GetAllPastDueInvoices(string division, string customerNumber);

        //List<EF.Invoice> GetAllPayableInvoices(string division, string customerNumber);

        List<EF.Invoice> GetInvoiceTransactoin(string division, string customerNumber, string invoiceNumber);

        List<EF.Invoice> GetMainInvoices(string division, string customerNumber);

		EF.Invoice GetInvoiceHeader(string division, string customerNumber, string invoiceNumber);

		long GetNextConfirmationId();

		void PayInvoice(PaymentTransaction payment);
    }
}
