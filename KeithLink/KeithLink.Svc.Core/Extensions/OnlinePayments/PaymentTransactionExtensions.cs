using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.OnlinePayments
{
	public static class PaymentTransactionExtensions
	{
		public static PaymentTransactionModel ToPaymentTransactionModel(this PaymentTransaction payment, KeithLink.Svc.Core.Models.Profile.Customer customer)
		{
			return new PaymentTransactionModel()
			{
				CustomerName = customer.CustomerName,
				CustomerNumber = customer.CustomerNumber,
				AccountNumber = payment.AccountNumber,
				ConfirmationId = payment.ConfirmationId,
				InvoiceNumber = payment.InvoiceNumber,
				PaymentAmount = payment.PaymentAmount,
				PaymentDate = payment.ScheduledPaymentDate,
				SubmittedDate = payment.PaymentDate,
				UserName = payment.UserName,
				Editable = DateTime.Now.Date < payment.ScheduledPaymentDate.Date
			};
		}
	}
}
