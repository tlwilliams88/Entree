﻿using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using Entree.Core.Enumerations.Messaging;
using Entree.Core.Interface.Messaging;
using Entree.Core.Models.Messaging.Provider;
using Entree.Core.Models.Messaging.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using AmazonSNS = Amazon.SimpleNotificationService;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class AmazonPushNotificationMessageProvider : IPushNotificationMessageProvider {
        #region attributes
        private readonly IEventLogRepository _eventLog;
        private IUserPushNotificationDeviceRepository _userPushNotificationDeviceRepository;
        private IUnitOfWork _unitOfWork;
        #endregion

        #region ctor
        public AmazonPushNotificationMessageProvider(IEventLogRepository eventLog, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, IUnitOfWork unitOfWork) {
            this._eventLog = eventLog;
            _userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region methods
        private void DisableDevices(List<Recipient> badRecipients) {
            foreach(Recipient recipient in badRecipients) {
                UserPushNotificationDevice device = _userPushNotificationDeviceRepository.ReadUserDevice(recipient.UserId, recipient.DeviceId, recipient.DeviceOS.Value);

                if(device != null) {
                    device.Enabled = false;
                    device.ModifiedUtc = DateTime.Now.ToUniversalTime();

                    _unitOfWork.SaveChanges();

                    StringBuilder msg = new StringBuilder();

                    msg.AppendLine("Disabling device.");
                    msg.AppendLine("DeviceId: {DeviceId}");
                    msg.AppendLine("DeviceOS: {DeviceOS}");
                    msg.AppendLine("ProviderEndPoint: {ProviderEndpoint}");

                    _eventLog.WriteInformationLog(msg.ToString().Inject(recipient));
                }
            }
        }

        public string RegisterRecipient(UserPushNotificationDevice userPushNotificationDevice)
        {
            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials(Configuration.AmazonSnsAccessKey, Configuration.AmazonSnsSecretKey),
                    Amazon.RegionEndpoint.USEast1);

            // first, check if device is registered
            bool doSnsRegistration = true;
            string currentArn = userPushNotificationDevice.ProviderEndpointId;
            if (!String.IsNullOrEmpty(currentArn))
            { // if we have an arn, read info from Amazon
                var response = client.GetEndpointAttributes(new AmazonSNS.Model.GetEndpointAttributesRequest()
                    {
                        EndpointArn = currentArn
                    });
                if (response.Attributes != null && response.Attributes.ContainsKey("Enabled") && response.Attributes.ContainsKey("Token"))
                {
                    string enabledStr = response.Attributes["Enabled"];
                    bool enabled = false;
                    bool.TryParse(enabledStr, out enabled);

                    string token = response.Attributes["Token"];
                    string savedToken = userPushNotificationDevice.ProviderToken;
                    bool hasBothSavedAndSnsToken = !String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(savedToken);

                    if (enabled && hasBothSavedAndSnsToken && token.Equals(savedToken))
                        doSnsRegistration = false;
                }
            }
            // then, register it if appropriate
            if (doSnsRegistration)
            {
                AmazonSNS.Model.CreatePlatformEndpointResponse response = null;
                if (userPushNotificationDevice.DeviceOS == Core.Enumerations.Messaging.DeviceOS.iOS)
                {
                    response = client.CreatePlatformEndpoint(new AmazonSNS.Model.CreatePlatformEndpointRequest()
                        {
                            Token = userPushNotificationDevice.ProviderToken,
                            PlatformApplicationArn = Configuration.AmazonSnsMobilePlatformAppArnIOS
                        });
                }
                else if (userPushNotificationDevice.DeviceOS == Core.Enumerations.Messaging.DeviceOS.Android)
                {
                    response = client.CreatePlatformEndpoint(new AmazonSNS.Model.CreatePlatformEndpointRequest()
                    {
                        Token = userPushNotificationDevice.ProviderToken,
                        PlatformApplicationArn = Configuration.AmazonSnsMobilePlatformAppArnAndroid
                    });
                }

                return response.EndpointArn;
            }
            return currentArn;
        }

        public void SendMessage(List<Recipient> recipients, Message message)
        {
			if (recipients == null || recipients.Count == 0)
				return;

            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials(Configuration.AmazonSnsAccessKey, Configuration.AmazonSnsSecretKey), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            BlockingCollection<Recipient> badRecipientList = new BlockingCollection<Recipient>();

            Parallel.ForEach(recipients, (recipient) => {
                try {
                    StringBuilder sendMsg = new StringBuilder();
                    sendMsg.AppendLine("Sending message to push recipient.");
                    sendMsg.AppendLine("UserId: {UserId}");
                    sendMsg.AppendLine("UserEmail: {UserEmail}");
                    sendMsg.AppendLine("CustomerNumber: {CustomerNumber}");
                    sendMsg.AppendLine("DeviceId: {DeviceId}");
                    sendMsg.AppendLine("DeviceOS: {DeviceOS}");
                    sendMsg.AppendLine("Channel: {Channel}");
                    sendMsg.AppendLine("ProviderEndPoint: {ProviderEndpoint}");

                    _eventLog.WriteInformationLog(sendMsg.ToString().Inject(recipient));

                    if(recipient.DeviceOS == DeviceOS.iOS) {
                        // format our message for apple
                        client.Publish(new AmazonSNS.Model.PublishRequest() { TargetArn = recipient.ProviderEndpoint, Message = message.MessageSubject }
                        );
                    } else if(recipient.DeviceOS == DeviceOS.Android) {
                        client.Publish(new AmazonSNS.Model.PublishRequest() {
                            TargetArn = recipient.ProviderEndpoint,
                            MessageStructure = "json",
                            Message = string.Format("{{\n\"GCM\": \"{{ \\\"data\\\": {{ \\\"message\\\": \\\"{0}\\\" }} }}\"\n}}", message.MessageSubject)
                        });

                    }
                } catch(Exception ex) {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendLine("Error sending message to push recipient.");
                    msg.AppendLine("UserId: {UserId}");
                    msg.AppendLine("UserEmail: {UserEmail}");
                    msg.AppendLine("CustomerNumber: {CustomerNumber}");
                    msg.AppendLine("DeviceId: {DeviceId}");
                    msg.AppendLine("DeviceOS: {DeviceOS}");
                    msg.AppendLine("Channel: {Channel}");
                    msg.AppendLine("ProviderEndPoint: {ProviderEndpoint}");

                    _eventLog.WriteErrorLog(msg.ToString().Inject(recipient), ex);

                    //if any of the strings we watch for is contained in the exception message, disable the device
                    if (Configuration.AmazonSnsMessagesToDisableOn.Any(ex.Message.ToLower().Contains)) {
                        badRecipientList.Add(recipient);
                    }
                }
            });

            if(badRecipientList.Count > 0) { DisableDevices(badRecipientList.ToList()); }
        }
        #endregion
    }
}
