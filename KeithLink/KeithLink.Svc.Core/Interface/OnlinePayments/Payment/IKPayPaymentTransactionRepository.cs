using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Payment
{
	public interface IKPayPaymentTransactionRepository
	{
		IEnumerable<Core.Models.OnlinePayments.Payment.EF.PaymentTransaction> ReadAll();
	}
}
