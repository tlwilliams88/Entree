using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using System.Data;

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

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetInvoiceTransactoin(string division, string customerNumber, string invoiceNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && i.CustomerNumber.Equals(customerNumber) && i.InvoiceNumber.StartsWith(invoiceNumber) && i.ItemSequence > 0).ToList();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetMainInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && i.CustomerNumber.Equals(customerNumber) && i.ItemSequence == 0).ToList();
        }
        
		public long GetNextConfirmationId()
		{
			//var returnCode = new SqlParameter();
			//returnCode.ParameterName = "@return_value";
			//returnCode.SqlDbType = SqlDbType.Int;
			//returnCode.Direction = ParameterDirection.Output;

			//_dbContext.Context.Database.ExecuteSqlCommand(
			//	"@return_value = [dbo].[procIncrementCounter]",
			//	returnCode);
			
			return 1;
		}

		public void PayInvoice(PaymentTransaction payment)
		{
			_dbContext.Context.Database.ExecuteSqlCommand(
				"procInsertTransaction @Division, @CustNum, @Invoice, @Account, @UserName, @Amount, @ConfId, @SchDate",
				new SqlParameter("@Division", payment.BranchId),
				new SqlParameter("@CustNum", payment.CustomerNumber),
				new SqlParameter("@Invoice", payment.InvoiceNumber),
				new SqlParameter("@Account", payment.AccountNumber),
				new SqlParameter("@UserName", payment.UserName),
				new SqlParameter("@Amount", payment.PaymentAmount),
				new SqlParameter("@ConfId", payment.ConfirmationId),
				new SqlParameter("@SchDate", payment.PaymentDate)
				);

		}
		#endregion
	}
}
