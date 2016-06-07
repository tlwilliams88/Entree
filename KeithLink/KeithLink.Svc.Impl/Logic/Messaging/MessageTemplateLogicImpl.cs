using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using System.Net.Mail;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class MessageTemplateLogicImpl : IMessageTemplateLogic
    {
        #region attributes
        private const string MESSAGE_TEMPLATE_HEADER = "NotifHeader";
        private readonly IMessageTemplateRepository emailTemplateRepository;
        #endregion

        #region ctor
        public MessageTemplateLogicImpl(IMessageTemplateRepository emailTemplateRepository)
        {
            this.emailTemplateRepository = emailTemplateRepository;
        }
        #endregion

        #region methods
        public MessageTemplateModel ReadForKey(string templateKey)
        {
            var template = emailTemplateRepository.Read(t => t.TemplateKey.Equals(templateKey)).FirstOrDefault();

            if (template == null)
                return null;

            return template.ToMessageTemplateModel();
        }
        public StringBuilder BuildHeader(string subject, Customer customer)
        {
            StringBuilder header = new StringBuilder();
            MessageTemplateModel templateHeader = ReadForKey(MESSAGE_TEMPLATE_HEADER);
            if ((subject != null) && (customer != null) && (customer.CustomerName != null) && (customer.CustomerNumber != null) && (customer.CustomerBranch != null))
            {
                header.Append(templateHeader.Body.Inject(new
                {
                    Subject = subject,
                    CustomerName = customer.CustomerName,
                    CustomerNumber = customer.CustomerNumber,
                    BranchID = customer.CustomerBranch
                }));
            }
            else if ((subject != null) && (customer != null) && (customer.CustomerName != null) && (customer.CustomerNumber != null))
            {
                header.Append(templateHeader.Body.Inject(new
                {
                    Subject = subject,
                    CustomerName = customer.CustomerName,
                    CustomerNumber = customer.CustomerNumber,
                    BranchID = ""
                }));
            }
            else if ((subject != null) && (customer != null) && (customer.CustomerNumber != null) && (customer.CustomerBranch != null))
            {
                header.Append(templateHeader.Body.Inject(new
                {
                    Subject = subject,
                    CustomerName = "",
                    CustomerNumber = customer.CustomerNumber,
                    BranchID = customer.CustomerBranch
                }));
            }
            else if (subject != null)
            {
                header.Append(templateHeader.Body.Inject(new
                {
                    Subject = subject,
                    CustomerName = "",
                    CustomerNumber = "",
                    BranchID = ""
                }));
            }
            return header;
        }
        #endregion
    }
}