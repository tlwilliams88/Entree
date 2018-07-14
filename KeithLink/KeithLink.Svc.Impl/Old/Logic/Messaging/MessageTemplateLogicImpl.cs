using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using Entree.Core.Extensions;
using Entree.Core.Interface.Email;
using Entree.Core.Models.Configuration;
using Entree.Core.Models.Profile;
using Entree.Core.Models.Messaging.Provider;
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

            header.Append(templateHeader.Body.Inject(new
            {
                Subject = subject,
                CustomerName = customer?.CustomerName,
                CustomerNumber = customer?.CustomerNumber,
                BranchID = customer?.CustomerBranch
            }));

            return header;
        }
        #endregion
    }
}