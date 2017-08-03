using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class ContractListChangesLogicImpl : IContractListChangesLogic
    {
        #region attributes
        private readonly ICatalogLogic _catalogLogic;
        private readonly ICustomerRepository _customerRepo;
        private readonly IEventLogRepository _log;
        private readonly IGenericQueueRepository _queueRepo;
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        private readonly IContractChangesRepository _contractChangesRepo;

        private const string MESSAGE_TEMPLATE_CONTRACTCHANGE = "ContractChangeNotice";
        private const string MESSAGE_TEMPLATE_CONTRACTCHANGEITEMS = "ContractChangeItem";
        #endregion

        #region ctor
        public ContractListChangesLogicImpl(ICatalogLogic catalogLogic, ICustomerRepository customerRepository,
                            IEventLogRepository eventLogRepository, IGenericQueueRepository queueRepository, IContractChangesRepository contractChangesRepo,
                            IMessageTemplateLogic messageTemplateLogic)
        {
            _catalogLogic = catalogLogic;
            _customerRepo = customerRepository;
            _log = eventLogRepository;
            _queueRepo = queueRepository;
            _contractChangesRepo = contractChangesRepo;
            _messageTemplateLogic = messageTemplateLogic;
        }
        #endregion

        #region methods
        public void ProcessContractChanges()
        {
            try
            {
                List<ContractChange> changes = _contractChangesRepo.ReadNextSet();

                while (changes != null && changes.Count > 0)
                {
                    BuildContractChangeNotifications(changes);

                    changes = _contractChangesRepo.ReadNextSet();
                }

            }
            catch (Exception outer)
            {
                _log.WriteErrorLog("Error processing contract change notifications", outer);
            }
        }

        private void BuildContractChangeNotifications(List<ContractChange> changes)
        {
            try
            {
                Customer customer = _customerRepo.GetCustomerByCustomerNumber(changes[0].CustomerNumber,
                    changes[0].BranchId);

                if (customer != null)
                {
                    var notifcation = BuildContractChangeNotification(changes, customer);
                    _queueRepo.PublishToDirectedExchange(notifcation.ToJson(),
                        Configuration.RabbitMQNotificationServer,
                        Configuration.RabbitMQNotificationUserNamePublisher,
                        Configuration.RabbitMQNotificationUserPasswordPublisher,
                        Configuration.RabbitMQVHostNotification,
                        Configuration.RabbitMQExchangeNotificationV2,
                        Constants.RABBITMQ_NOTIFICATION_HASNEWS_ROUTEKEY);
                    _log.WriteInformationLog(string.Format("Published to notification exchange, {0}",
                        notifcation.ToJson()));
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error creating contract change notification", ex);
            }

            _contractChangesRepo.Update(changes[0].ParentList_Id, true);
        }

        private HasNewsNotification BuildContractChangeNotification(List<ContractChange> changes, Customer customer)
        {
            StringBuilder header =
                _messageTemplateLogic.BuildHeader("Some items on your contract have changed", customer);

            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_CONTRACTCHANGE);

            StringBuilder itemsContent = new StringBuilder();
            foreach (ContractChange change in changes)
            {
                AddContractChangeToNotification(change, itemsContent);
            }

            HasNewsNotification notifcation = new HasNewsNotification()
            {
                CustomerNumber = changes[0].CustomerNumber,
                BranchId = changes[0].BranchId,
                Subject = template.Subject.Inject(new
                {
                    CustomerNumber = customer.CustomerNumber,
                    CustomerName = customer.CustomerName
                }),
                Notification = template.Body.Inject(new
                {
                    NotifHeader = header.ToString(),
                    ContractChangeItems = itemsContent.ToString()
                })
            };
            return notifcation;
        }

        private void AddContractChangeToNotification(ContractChange change, StringBuilder itemsContent)
        {
            itemsContent.Append(ToContractChangeString(change, TryToFindProductForContractChange(change)));
        }

        private string ToContractChangeString(ContractChange change, Product itemdetail)
        {
            MessageTemplateModel itemTemplate =
                _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_CONTRACTCHANGEITEMS);
            return itemTemplate.Body.Inject(new
            {
                Status =
                (change.Status.Equals("Added"))
                    ? "<font color=green>" + change.Status + "</font>"
                    : "<font color=red>" + change.Status + "</font>",
                ProductNumber = change.ItemNumber,
                ProductDescription = (itemdetail != null) ? itemdetail.Name : "",
                Brand = (itemdetail != null) ? itemdetail.BrandExtendedDescription : "",
                Pack = (itemdetail != null) ? itemdetail.Pack : "",
                Size = (itemdetail != null) ? itemdetail.Size : ""
            });
        }

        private Product TryToFindProductForContractChange(ContractChange change)
        {
            Product itemdetail = null;
            try
            {
                ProductsReturn tempProducts = null;
                List<string> tmp = new List<string>();
                tmp.Add(change.ItemNumber);
                tempProducts = _catalogLogic.GetProductsByIds(change.BranchId, tmp);
                if (tempProducts != null)
                {
                    itemdetail =
                        tempProducts.Products.Where(p => p.ItemNumber == change.ItemNumber).FirstOrDefault();
                }
            }
            catch
            {
                _log.WriteWarningLog(string.Format("Failure to look up item {0}, branch {1}", change.ItemNumber,
                    change.BranchId));
            }
            return itemdetail;
        }
        #endregion
    }
}
