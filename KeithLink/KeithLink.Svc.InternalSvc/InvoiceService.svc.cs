using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select InvoiceService.svc or InvoiceService.svc.cs at the Solution Explorer and start debugging.
	public class InvoiceService : IInvoiceService {
        #region attributes
        private IInternalInvoiceLogic invoiceLogic;
        private IKPayInvoiceRepository _kpayRepo;
        #endregion

        #region ctor
        public InvoiceService(IInternalInvoiceLogic invoiceLogic, IKPayInvoiceRepository kpayInvoiceRepo) {
			this.invoiceLogic = invoiceLogic;
            _kpayRepo = kpayInvoiceRepo;
		}
        #endregion

        #region methods
        private string GetDivision(string branchId) {
            if (branchId.Length == 5) {
                return branchId;
            } else if (branchId.Length == 3) {
                switch (branchId.ToUpper()) {
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
            } else {
                return null;
            }
        }

		public InvoiceModel ReadInvoice(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return invoiceLogic.ReadInvoice(user, catalogInfo, Id);
		}

        public List<InvoiceModel> ReadInvoices(UserProfile user, UserSelectedContext catalogInfo)
		{
            // get the invoices from app_data
			var returnvalue =  invoiceLogic.ReadInvoices(user, catalogInfo);
            // get the invoices from kpay
            List<Core.Models.OnlinePayments.Invoice.EF.Invoice> kpayInvoices = _kpayRepo.GetMainInvoices(GetDivision(catalogInfo.BranchId), catalogInfo.CustomerId);

            // if there are kpay invoices
            if (kpayInvoices.Count > 0) {
                // look for matches between the two and flag them as payable if they are found
                foreach (var invoice in returnvalue) {
                    invoice.IsPayable = (kpayInvoices.Where(i => i.InvoiceNumber.StartsWith(invoice.InvoiceNumber)).Count() > 0);  
                }
            }

			return returnvalue;
		}

		public TermModel ReadTermInformation(string branchId, string termCode)
		{
			return invoiceLogic.ReadTermInformation(branchId, termCode);
		}
        #endregion
	}
}
