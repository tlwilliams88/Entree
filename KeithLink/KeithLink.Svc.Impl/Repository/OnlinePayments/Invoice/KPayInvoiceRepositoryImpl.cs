using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using System.Data;
using System.Linq.Expressions;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice {
    public class KPayInvoiceRepositoryImpl : IKPayInvoiceRepository {
        #region attributes
        private readonly IKPayDBContext _dbContext;
        #endregion

        #region ctor
        public KPayInvoiceRepositoryImpl(IKPayDBContext kpayDBContext) {
            _dbContext = kpayDBContext;
        }
        #endregion

        #region methods
        public void DeleteInvoice(string division, string customerNumber, string invoiceNumber) {
            SqlParameter branchParm = new SqlParameter("@Division", division);
            SqlParameter custParm = new SqlParameter("@CustNum", customerNumber);
            SqlParameter invParm = new SqlParameter("@Invoice", invoiceNumber.PadRight(30));

            _dbContext.Context.Database.ExecuteSqlCommand(
                    "procDeleteInvoice @Division, @CustNum, @Invoice",
                    branchParm,
                    custParm,
                    invParm
                );
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetAllOpenInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && 
                                                  i.CustomerNumber.Equals(customerNumber) && 
                                                  i.ItemSequence == 0 && 
                                                  i.ItemLine == 0 && 
                                                  i.InvoiceStatus.Equals("o", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetAllPaidInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && 
                                                  i.CustomerNumber.Equals(customerNumber) && 
                                                  i.ItemSequence == 0 &&
                                                  i.ItemLine == 0 && 
                                                  i.InvoiceStatus.Equals("c", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetAllPastDueInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && 
                                                  i.CustomerNumber.Equals(customerNumber) && 
                                                  i.ItemSequence == 0 &&
                                                  i.ItemLine == 0 && 
                                                  i.InvoiceStatus.Equals("o", StringComparison.InvariantCultureIgnoreCase) &&
                                                  i.DueDate < DateTime.Now).ToList();
        }

        public Core.Models.OnlinePayments.Invoice.EF.Invoice GetInvoiceHeader(string division, string customerNumber, string invoiceNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) &&
                                                  i.CustomerNumber.Equals(customerNumber) &&
                                                  i.ItemSequence == 0 &&
                                                  i.InvoiceNumber.Equals(invoiceNumber)).FirstOrDefault();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetInvoiceTransactoin(string division, string customerNumber, string invoiceNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && 
                                                  i.CustomerNumber.Equals(customerNumber) && 
                                                  i.InvoiceNumber.StartsWith(invoiceNumber) &&
												  i.ItemSequence > 0).ToList();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetMainInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && 
                                                  i.CustomerNumber.Equals(customerNumber) && 
                                                  i.ItemSequence == 0 && 
                                                  i.ItemLine == 0).ToList();
        }

		public long GetNextConfirmationId()
		{
			var returnCode = new SqlParameter();
			returnCode.ParameterName = "@ReturnCode";
			returnCode.SqlDbType = SqlDbType.Int;
			returnCode.Direction = ParameterDirection.Output;

			// assign the return code to the new output parameter and pass it to the sp
			_dbContext.Context.Database.ExecuteSqlCommand("EXEC @ReturnCode = [dbo].[procIncrementCounter]", returnCode);
			
			return (int)returnCode.Value;
		}

		public void PayInvoice(PaymentTransaction payment)
		{
			_dbContext.Context.Database.ExecuteSqlCommand(
				"procInsertTransaction @Division, @CustNum, @Invoice, @Account, @UserName, @Amount, @ConfId, @SchDate",
				new SqlParameter("@Division", payment.Division),
				new SqlParameter("@CustNum", payment.CustomerNumber),
				new SqlParameter("@Invoice", payment.InvoiceNumber),
				new SqlParameter("@Account", payment.AccountNumber),
				new SqlParameter("@UserName", payment.UserName),
				new SqlParameter("@Amount", payment.PaymentAmount),
				new SqlParameter("@ConfId", payment.ConfirmationId),
				new SqlParameter("@SchDate", payment.PaymentDate)
				);
		}

        public void MarkInvoiceAsPaid(string division, string customerNumber, string invoiceNumber) {
            _dbContext.Context.Database.ExecuteSqlCommand(
                "procMarkInvoiceAsPaid @Division, @CustNum, @Invoice",
                new SqlParameter("@Division", division),
                new SqlParameter("@CustNum", customerNumber),
                new SqlParameter("@Invoice", invoiceNumber)
                );
        }
			

		public IEnumerable<Core.Models.OnlinePayments.Invoice.EF.Invoice> ReadAll()
		{
			return this._dbContext.Invoices;
		}

        public IEnumerable<Core.Models.OnlinePayments.Invoice.EF.InvoiceHeader> ReadAllHeaders() {
            return this._dbContext.InvoiceHeaders;
        }

		#endregion


		
	}
}
