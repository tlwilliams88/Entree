﻿using Autofac;

using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Profile;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Order
    {
    [TestClass]
    public class OrderNotificationTests {
        #region attrbitues
        private IEventLogRepository _log;
        private IUserProfileLogic _userProfileLogic;
        private ICustomerRepository _cust;
        #endregion

        #region ctor
        public OrderNotificationTests() {
			var container = DependencyMap.Build();

            _log = container.Resolve<IEventLogRepository>();
            _cust = container.Resolve<ICustomerRepository>();
            _userProfileLogic = container.Resolve<IUserProfileLogic>();
        }
        #endregion

        #region methods
        private string GetSampleOrderMessage() {
            return @"{""notificationtype"":32,""orders"":[{""orderid"":""33276944"", ""scheduledtime"":""2015-01-07T07:02:00-06:00"", ""estimatedtime"":""2015-01-07T06:41:00-06:00"", ""routeid"":""0335"", ""stopnumber"":""3"", ""outofsequence"":""true""},{""orderid"":""33277236"", ""scheduledtime"":""2015-01-07T08:23:00-06:00"", ""estimatedtime"":""2015-01-07T09:09:00-06:00"", ""routeid"":""0335"", ""stopnumber"":""7"", ""outofsequence"":""true""},{""orderid"":""33277055"", ""scheduledtime"":""2015-01-07T09:39:00-06:00"", ""estimatedtime"":""2015-01-07T11:52:00-06:00"", ""routeid"":""0338"", ""stopnumber"":""9"", ""outofsequence"":""true""},{""orderid"":""33277295"", ""scheduledtime"":""2015-01-07T10:19:00-06:00"", ""estimatedtime"":""2015-01-07T11:42:00-06:00"", ""routeid"":""0338"", ""stopnumber"":""11"", ""outofsequence"":""true""},{""orderid"":""33277119"", ""scheduledtime"":""2015-01-07T12:28:00-06:00"", ""estimatedtime"":""2015-01-07T12:52:00-06:00"", ""routeid"":""0338"", ""stopnumber"":""14"", ""outofsequence"":""true""},{""orderid"":""33277032"", ""scheduledtime"":""2015-01-07T13:21:00-06:00"", ""estimatedtime"":""2015-01-07T08:39:00-06:00"", ""routeid"":""0338"", ""stopnumber"":""16"", ""outofsequence"":""true""},{""orderid"":""33276898"", ""scheduledtime"":""2015-01-07T09:24:00-06:00"", ""estimatedtime"":""2015-01-07T08:33:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""4"", ""outofsequence"":""true""},{""orderid"":""33276967"", ""scheduledtime"":""2015-01-07T09:41:00-06:00"", ""estimatedtime"":""2015-01-07T08:50:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""5"", ""outofsequence"":""true""},{""orderid"":""33277065"", ""scheduledtime"":""2015-01-07T10:17:00-06:00"", ""estimatedtime"":""2015-01-07T09:26:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""6"", ""outofsequence"":""true""},{""orderid"":""33276865"", ""scheduledtime"":""2015-01-07T11:56:00-06:00"", ""estimatedtime"":""2015-01-07T12:00:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""9"", ""outofsequence"":""true""},{""orderid"":""33276830"", ""scheduledtime"":""2015-01-07T12:43:00-06:00"", ""estimatedtime"":""2015-01-07T13:13:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""11"", ""outofsequence"":""true""},{""orderid"":""33276831"", ""scheduledtime"":""2015-01-07T12:43:00-06:00"", ""estimatedtime"":""2015-01-07T13:13:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""11"", ""outofsequence"":""true""},{""orderid"":""33276817"", ""scheduledtime"":""2015-01-07T13:21:00-06:00"", ""estimatedtime"":""2015-01-07T13:51:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""12"", ""outofsequence"":""true""},{""orderid"":""33276835"", ""scheduledtime"":""2015-01-07T13:21:00-06:00"", ""estimatedtime"":""2015-01-07T13:51:00-06:00"", ""routeid"":""0347"", ""stopnumber"":""12"", ""outofsequence"":""true""},{""orderid"":""33277029"", ""scheduledtime"":""2015-01-07T14:55:00-06:00"", ""estimatedtime"":""2015-01-07T13:25:00-06:00"", ""routeid"":""0348"", ""stopnumber"":""13"", ""outofsequence"":""true""},{""orderid"":""33276970"", ""scheduledtime"":""2015-01-07T16:21:00-06:00"", ""estimatedtime"":""2015-01-07T13:15:00-06:00"", ""routeid"":""0348"", ""stopnumber"":""16"", ""outofsequence"":""true""},{""orderid"":""33277284"", ""scheduledtime"":""2015-01-07T16:58:00-06:00"", ""estimatedtime"":""2015-01-07T13:52:00-06:00"", ""routeid"":""0348"", ""stopnumber"":""17"", ""outofsequence"":""true""},{""orderid"":""33277143"", ""scheduledtime"":""2015-01-07T06:38:00-06:00"", ""estimatedtime"":""2015-01-07T05:59:00-06:00"", ""routeid"":""0353"", ""stopnumber"":""3"", ""outofsequence"":""true""},{""orderid"":""33276787"", ""scheduledtime"":""2015-01-07T10:20:00-06:00"", ""estimatedtime"":""2015-01-07T10:38:00-06:00"", ""routeid"":""0353"", ""stopnumber"":""11"", ""outofsequence"":""true""},{""orderid"":""33277264"", ""scheduledtime"":""2015-01-07T12:46:00-06:00"", ""estimatedtime"":""2015-01-07T12:27:00-06:00"", ""routeid"":""0353"", ""stopnumber"":""18"", ""outofsequence"":""true""},{""orderid"":""33277060"", ""scheduledtime"":""2015-01-07T10:20:00-06:00"", ""estimatedtime"":""2015-01-07T13:04:00-06:00"", ""routeid"":""0355"", ""stopnumber"":""8"", ""outofsequence"":""true""},{""orderid"":""33277185"", ""scheduledtime"":""2015-01-07T11:31:00-06:00"", ""estimatedtime"":""2015-01-07T13:09:00-06:00"", ""routeid"":""0355"", ""stopnumber"":""10"", ""outofsequence"":""true""},{""orderid"":""41733307"", ""scheduledtime"":""2015-01-07T11:31:00-06:00"", ""estimatedtime"":""2015-01-07T13:09:00-06:00"", ""routeid"":""0355"", ""stopnumber"":""10"", ""outofsequence"":""true""},{""orderid"":""33277252"", ""scheduledtime"":""2015-01-07T12:00:00-06:00"", ""estimatedtime"":""2015-01-07T13:38:00-06:00"", ""routeid"":""0355"", ""stopnumber"":""11"", ""outofsequence"":""true""},{""orderid"":""33276996"", ""scheduledtime"":""2015-01-07T11:49:00-06:00"", ""estimatedtime"":""2015-01-07T13:04:00-06:00"", ""routeid"":""0366"", ""stopnumber"":""7"", ""outofsequence"":""false""},{""orderid"":""33277228"", ""scheduledtime"":""2015-01-07T12:29:00-06:00"", ""estimatedtime"":""2015-01-07T13:44:00-06:00"", ""routeid"":""0366"", ""stopnumber"":""8"", ""outofsequence"":""false""},{""orderid"":""33277020"", ""scheduledtime"":""2015-01-07T13:35:00-06:00"", ""estimatedtime"":""2015-01-07T14:50:00-06:00"", ""routeid"":""0366"", ""stopnumber"":""9"", ""outofsequence"":""false""},{""orderid"":""33277254"", ""scheduledtime"":""2015-01-07T14:15:00-06:00"", ""estimatedtime"":""2015-01-07T15:30:00-06:00"", ""routeid"":""0366"", ""stopnumber"":""10"", ""outofsequence"":""false""},{""orderid"":""33277275"", ""scheduledtime"":""2015-01-07T12:19:00-06:00"", ""estimatedtime"":""2015-01-07T13:24:00-06:00"", ""routeid"":""0368"", ""stopnumber"":""14"", ""outofsequence"":""true""},{""orderid"":""33277279"", ""scheduledtime"":""2015-01-07T13:23:00-06:00"", ""estimatedtime"":""2015-01-07T13:09:00-06:00"", ""routeid"":""0368"", ""stopnumber"":""16"", ""outofsequence"":""true""},{""orderid"":""33277062"", ""scheduledtime"":""2015-01-07T10:56:00-06:00"", ""estimatedtime"":""2015-01-07T13:24:00-06:00"", ""routeid"":""0369"", ""stopnumber"":""9"", ""outofsequence"":""false""},{""orderid"":""33277094"", ""scheduledtime"":""2015-01-07T11:10:00-06:00"", ""estimatedtime"":""2015-01-07T13:38:00-06:00"", ""routeid"":""0369"", ""stopnumber"":""10"", ""outofsequence"":""false""},{""orderid"":""33276916"", ""scheduledtime"":""2015-01-07T11:23:00-06:00"", ""estimatedtime"":""2015-01-07T13:51:00-06:00"", ""routeid"":""0369"", ""stopnumber"":""11"", ""outofsequence"":""false""},{""orderid"":""33277041"", ""scheduledtime"":""2015-01-07T09:09:00-06:00"", ""estimatedtime"":""2015-01-07T09:58:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""6"", ""outofsequence"":""true""},{""orderid"":""33277305"", ""scheduledtime"":""2015-01-07T11:02:00-06:00"", ""estimatedtime"":""2015-01-07T11:55:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""8"", ""outofsequence"":""true""},{""orderid"":""33277162"", ""scheduledtime"":""2015-01-07T12:46:00-06:00"", ""estimatedtime"":""2015-01-07T13:39:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""9"", ""outofsequence"":""true""},{""orderid"":""33277240"", ""scheduledtime"":""2015-01-07T14:39:00-06:00"", ""estimatedtime"":""2015-01-07T13:15:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""12"", ""outofsequence"":""true""},{""orderid"":""33277289"", ""scheduledtime"":""2015-01-07T15:27:00-06:00"", ""estimatedtime"":""2015-01-07T14:03:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""13"", ""outofsequence"":""true""},{""orderid"":""33277291"", ""scheduledtime"":""2015-01-07T15:27:00-06:00"", ""estimatedtime"":""2015-01-07T14:03:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""13"", ""outofsequence"":""true""},{""orderid"":""33276960"", ""scheduledtime"":""2015-01-07T16:13:00-06:00"", ""estimatedtime"":""2015-01-07T14:49:00-06:00"", ""routeid"":""0372"", ""stopnumber"":""14"", ""outofsequence"":""true""}]}";
        }

        public void ReadNotification() {
            // temp test for queue stuff
            //KeithLink.Svc.Impl.Repository.Queue.GenericQueueRepositoryImpl queueRepo =
            //    new Impl.Repository.Queue.GenericQueueRepositoryImpl();
            //queueRepo.PublishToQueue("gstestitem", "corpkecdev1", "notifypub", "notifypasspub", "notifications_notify", "bek_commerce_notifications");
            /////////////////////////////////////
			//string notificationMsg = GetSampleOrderMessage();

			//KeithLink.Svc.Core.Models.Messaging.Queue.BaseNotification
			//	notification = KeithLink.Svc.Core.Extensions.Messaging.NotificationExtension.Deserialize(notificationMsg);

			//Assert.IsTrue(notification.NotificationType == Core.Enumerations.Messaging.NotificationType.Eta);

			//KeithLink.Svc.Core.Models.Messaging.Queue.EtaNotification etaNotification =
			//	(KeithLink.Svc.Core.Models.Messaging.Queue.EtaNotification)notification;

			//Assert.IsTrue(etaNotification.Orders.Count > 0);

			//DateTime dt = etaNotification.Orders[0].EstimatedTime.ToDateTime().Value;
			//dt = dt.ToCentralTime();
			//string date = dt.ToShortDateString();
			//string time = dt.ToShortTimeString();
			//string timeZoneName = dt.CentralTimeZoneName();
			//string all = date + ", " + time;
			//EtaNotificationHandlerImpl etaHandler = new EtaNotificationHandlerImpl(
			//	_log,
			//	_userProfileLogic,
			//	null,
			//	_cust,
			//	null,
			//	null,
			//	null,
			//	new Svc.Impl.Logic.Messaging.MessageTemplateLogicImpl(
			//		new Svc.Impl.Repository.Email.MessageTemplateRepositoryImpl(new UnitOfWork(new EventLogRepositoryImpl("Entree Tests")))),
			//		null
			//	);

			//// create  a list of fake order histories
			//List<Core.Models.Orders.History.EF.OrderHistoryHeader> orderHistoryHeaders = new List<Core.Models.Orders.History.EF.OrderHistoryHeader>();
			//Random r = new Random();
			//foreach (OrderEta oe in etaNotification.Orders)
			//{
			//	Core.Models.Orders.History.EF.OrderHistoryHeader h = new Core.Models.Orders.History.EF.OrderHistoryHeader() { InvoiceNumber = oe.OrderId, BranchId = "FDF" };
			//	h.OrderDetails = new List<Core.Models.Orders.History.EF.OrderHistoryDetail>();
			//	for (int i = 0; i < (r.Next(1, 10)); i++)
			//	{
			//		h.OrderDetails.Add(new Core.Models.Orders.History.EF.OrderHistoryDetail() { LineNumber = i, ShippedQuantity = i + 2, OrderQuantity = i + 2 });
			//	}
			//	orderHistoryHeaders.Add(h);
			//}

			//PrivateObject o = new PrivateObject(etaHandler, new PrivateType(typeof(EtaNotificationHandlerImpl)));
			//List<object> callParams = new List<object>();
			//callParams.Add(etaNotification.Orders);
			//callParams.Add(orderHistoryHeaders);
			//callParams.Add(new Core.Models.Profile.Customer() { CustomerNumber = "12345", CustomerName = "ABC Foods" });
			//List<Type> callParamTypes = new List<Type>();
			//callParamTypes.Add(typeof(IEnumerable<OrderEta>));
			//callParamTypes.Add(typeof(IEnumerable<Core.Models.Orders.History.EF.OrderHistoryHeader>));
			//callParamTypes.Add(typeof(Svc.Core.Models.Profile.Customer));

			//o.Invoke("GetEmailMessageForNotification",
			//	System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod,
			//	callParamTypes.ToArray(),
			//	callParams.ToArray());
        }
        #endregion
    }
}
