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
            PaymentTransactionModel retVal = new PaymentTransactionModel();

			retVal.CustomerName = customer.CustomerName;
			retVal.CustomerNumber = customer.CustomerNumber;
			retVal.AccountNumber = payment.AccountNumber;
			retVal.ConfirmationId = payment.ConfirmationId;
			retVal.InvoiceNumber = payment.InvoiceNumber;
			retVal.PaymentAmount = payment.PaymentAmount;
			retVal.PaymentDate = payment.ScheduledPaymentDate;
			retVal.SubmittedDate = payment.PaymentDate;
			retVal.BranchId = string.IsNullOrEmpty(payment.Division) ? string.Empty : payment.Division.Substring(0,3);
			retVal.UserName = payment.UserName;

            if (payment.ScheduledPaymentDate.HasValue) {
                DateTime cutOffTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                   14, 0, 0, DateTimeKind.Local);

                retVal.Editable = payment.ScheduledPaymentDate.Value < cutOffTime;
            } else {
                retVal.Editable = true;
            }

            return retVal;
		}
	}
}
