﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.OnlinePayments.Payment
{
	public interface IKPayPaymentTransactionRepository
	{
		IEnumerable<Core.Models.OnlinePayments.Payment.EF.PaymentTransaction> ReadAll();
        IEnumerable<Core.Models.OnlinePayments.Payment.EF.PaymentTransaction> ReadAllByCustomer( string customerNumber, string division );
	}
}
