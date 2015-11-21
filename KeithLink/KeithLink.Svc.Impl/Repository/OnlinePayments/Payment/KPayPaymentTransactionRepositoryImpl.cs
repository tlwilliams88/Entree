using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Payment
{
	public class KPayPaymentTransactionRepositoryImpl: IKPayPaymentTransactionRepository
	{
		private readonly IKPayDBContext _dbContext;
	

		public KPayPaymentTransactionRepositoryImpl(IKPayDBContext dbContext)
		{
			this._dbContext = dbContext;
		}

		public IEnumerable<Core.Models.OnlinePayments.Payment.EF.PaymentTransaction> ReadAll()
		{
			return this._dbContext.PaymentTransactions;
		}

        public IEnumerable<Core.Models.OnlinePayments.Payment.EF.PaymentTransaction> ReadAllByCustomer( string customerNumber, string division ) {
            return this._dbContext.PaymentTransactions.Where( x => x.CustomerNumber.Equals( customerNumber ) && x.Division.Equals( division ) );
        }
	}
}
