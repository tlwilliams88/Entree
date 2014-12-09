using KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Invoice {
    public interface IKPayInvoiceRepository {
        void DeleteInvoice(string division, string customerNumber, string invoiceNumber);

        List<Models.OnlinePayments.Invoice.EF.Invoice> GetInvoiceTransactoin(string division, string customerNumber, string invoiceNumber);

        List<Models.OnlinePayments.Invoice.EF.Invoice> GetMainInvoices(string division, string customerNumber);

		Core.Models.OnlinePayments.Invoice.EF.Invoice GetInvoiceHeader(string division, string customerNumber, string invoiceNumber);

		long GetNextConfirmationId();

		void PayInvoice(PaymentTransaction payment);
    }
}
