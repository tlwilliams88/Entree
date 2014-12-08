using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalOnlinePaymentLogicImpl: IOnlinePaymentsLogic
	{
		private readonly IKPayInvoiceRepository _invoiceRepo;
		private readonly ICustomerBankRepository _bankRepo;
		private readonly IOrderHistoryHeaderRepsitory _orderHistoryRepo;
		private readonly ICatalogLogic _catalogLogic;
		
		public InternalOnlinePaymentLogicImpl(IKPayInvoiceRepository invoiceRepo, ICustomerBankRepository bankRepo, IOrderHistoryHeaderRepsitory orderHistoryrepo,
			ICatalogLogic catalogLogic)
		{
			this._invoiceRepo = invoiceRepo;
			this._bankRepo = bankRepo;
			this._orderHistoryRepo = orderHistoryrepo;
			this._catalogLogic = catalogLogic;
		}

		public List<InvoiceModel> GetOpenInvoiceHeaders(UserSelectedContext userContext)
		{
			List<EFInvoice.Invoice> kpayInvoices = _invoiceRepo.GetMainInvoices(GetDivision(userContext.BranchId), userContext.CustomerId);
			List<InvoiceModel> returnInvoices = kpayInvoices.Select(i => i.ToInvoiceModel()).ToList();

			foreach (var inv in returnInvoices.Where(i => i.Type == InvoiceType.Invoice))
			{
				//TODO: add check to see if customer is KPay customer
				inv.IsPayable = true;
				if (inv.DueDate <= DateTime.Now)
				{
					inv.Status = InvoiceStatus.PastDue;
					inv.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
				}
			}

			return returnInvoices;
		}

		public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments)
		{
			var confId = _invoiceRepo.GetNextConfirmationId();

			foreach (var payment in payments)
				_invoiceRepo.PayInvoice(new Core.Models.OnlinePayments.Payment.EF.PaymentTransaction()
				{
					AccountNumber = payment.AccountNumber,
					BranchId = GetDivision(userContext.BranchId),
					ConfirmationId = confId,
					CustomerNumber = userContext.CustomerId,
					InvoiceNumber = payment.InvoiceNumber,
					PaymentAmount = payment.PaymentAmount,
					PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value : DateTime.Now,
					UserName = emailAddress
				});
		}

		public List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext)
		{
			List<EFCustomer.CustomerBank> bankEntities = _bankRepo.GetAllCustomerBanks(GetDivision(userContext.BranchId), userContext.CustomerId);

			List<CustomerBank> banks = new List<CustomerBank>();

			foreach (EFCustomer.CustomerBank entity in bankEntities)
			{
				if (entity != null)
				{
					CustomerBank bank = new CustomerBank();
					bank.Parse(entity);

					banks.Add(bank);
				}
			}

			return banks;
		}
		

		public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber)
		{
			EFCustomer.CustomerBank bankEntity = _bankRepo.GetBankAccount(GetDivision(userContext.BranchId), userContext.CustomerId, accountNumber);

			if (bankEntity == null)
				return null;
			else
			{
				CustomerBank bank = new CustomerBank();
				bank.Parse(bankEntity);

				return bank;
			}
		}


		public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber)
		{
			_invoiceRepo.DeleteInvoice(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
		}


		public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber)
		{
			var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);

			if (kpayInvoiceHeader == null) //Invoice not found
				return null;

			//Convert to invoice model
			var invoiceModel = kpayInvoiceHeader.ToInvoiceModel();

			if (invoiceModel.DueDate < DateTime.Now)
			{
				invoiceModel.Status = InvoiceStatus.PastDue;
				invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
			}

			//Get transactions
			var transactions = _invoiceRepo.GetInvoiceTransactoin(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
			invoiceModel.Transactions = transactions.Select(t => t.ToTransationModel()).ToList();

			//Retrieve invoice details, from order history

			var details = _orderHistoryRepo.Read(o => o.InvoiceNumber.Equals(invoiceNumber), d => d.OrderDetails).FirstOrDefault();

			if (details != null && details.OrderDetails != null)
			{
				invoiceModel.Items = details.OrderDetails.Select(d => d.ToInvoiceItem()).ToList();
			}
			//TODO: add check to see if customer is KPay customer
			invoiceModel.IsPayable = true;

			//look up product details
			LookupProductDetails(invoiceModel, userContext);
			
			return invoiceModel;
		}

		#region Helper Methods

		private string GetDivision(string branchId)
		{
			if (branchId.Length == 5)
			{
				return branchId;
			}
			else if (branchId.Length == 3)
			{
				switch (branchId.ToUpper())
				{
					case "FAM":
						return "FAM04";
					case "FAQ":
						return "FAQ08";
					case "FAR":
						return "FAR09";
					case "FDF":
						return "FDF01";
					case "FHS":
						return "FHS03";
					case "FLR":
						return "FLR05";
					case "FOK":
						return "FOK06";
					case "FSA":
						return "FSA07";
					default:
						return null;
				}
			}
			else
			{
				return null;
			}
		}


		private void LookupProductDetails(InvoiceModel invoiceItem, UserSelectedContext catalogInfo)
		{
			if (invoiceItem.Items == null || invoiceItem.Items.Count == 0)
				return;

			var products = _catalogLogic.GetProductsByIds(invoiceItem.BranchId, invoiceItem.Items.Select(i => i.ItemNumber).Distinct().ToList());

			Parallel.ForEach(invoiceItem.Items, invoiceDetailItem =>
			{
				var prod = products.Products.Where(p => p.ItemNumber.Equals(invoiceDetailItem.ItemNumber)).FirstOrDefault();
				if (prod != null)
				{
					invoiceDetailItem.Name = prod.Name;
					invoiceDetailItem.Description = prod.Description;
					invoiceDetailItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					invoiceDetailItem.Brand = prod.BrandExtendedDescription;
					invoiceDetailItem.BrandExtendedDescription = prod.BrandExtendedDescription;
					invoiceDetailItem.ReplacedItem = prod.ReplacedItem;
					invoiceDetailItem.ReplacementItem = prod.ReplacementItem;
					invoiceDetailItem.NonStock = prod.NonStock;
					invoiceDetailItem.ChildNutrition = prod.ChildNutrition;
					invoiceDetailItem.CatchWeight = prod.CatchWeight;
					invoiceDetailItem.TempZone = prod.TempZone;
					invoiceDetailItem.ItemClass = prod.ItemClass;
					invoiceDetailItem.CategoryId = prod.CategoryId;
					invoiceDetailItem.CategoryName = prod.CategoryName;
					invoiceDetailItem.UPC = prod.UPC;
					invoiceDetailItem.VendorItemNumber = prod.VendorItemNumber;
					invoiceDetailItem.Cases = prod.Cases;
					invoiceDetailItem.Kosher = prod.Kosher;
					invoiceDetailItem.ManufacturerName = prod.ManufacturerName;
					invoiceDetailItem.ManufacturerNumber = prod.ManufacturerNumber;
					invoiceDetailItem.Nutritional = new Nutritional()
					{
						CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
						GrossWeight = prod.Nutritional.GrossWeight,
						HandlingInstructions = prod.Nutritional.HandlingInstructions,
						Height = prod.Nutritional.Height,
						Length = prod.Nutritional.Length,
						Ingredients = prod.Nutritional.Ingredients,
						Width = prod.Nutritional.Width
					};

				}				
			});

		}

		#endregion


		

	}
}
