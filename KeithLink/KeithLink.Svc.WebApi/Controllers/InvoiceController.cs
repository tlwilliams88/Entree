//using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class InvoiceController : BaseController {
        #region attributes
        private readonly IOnlinePaymentServiceRepository _repo;
        #endregion

        #region ctor
        #endregion
        public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentServiceRepository invoiceRepository) : base(profileLogic) {
            _repo = invoiceRepository;
		}

        #region methods
        [HttpGet]
        [ApiKeyedRoute("invoice/")]
        public List<InvoiceModel> Invoice() {
            return _repo.GetOpenInvoiceHeaders(SelectedUserContext);
        }

        [HttpGet]
        [ApiKeyedRoute("invoice/{invoiceNumber}")]
        public List<InvoiceModel> InvoiceTransactions(string invoiceNumber) {
            return _repo.GetInvoiceTransactions(SelectedUserContext, invoiceNumber);
        }
        #endregion
    }
}
