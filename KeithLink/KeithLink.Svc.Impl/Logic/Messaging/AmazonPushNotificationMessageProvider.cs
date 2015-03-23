using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.EF;
using AmazonSNS = Amazon.SimpleNotificationService;


namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class AmazonPushNotificationMessageProvider : IPushNotificationMessageProvider
    {
        KeithLink.Common.Core.Logging.IEventLogRepository eventLog;

        public AmazonPushNotificationMessageProvider(KeithLink.Common.Core.Logging.IEventLogRepository eventLog)
        {
            this.eventLog = eventLog;
        }

		public void SendMessage(List<Recipient> recipients, Message message)
        {
			if (recipients == null)
				return;

            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials(Configuration.AmazonSnsAccessKey, Configuration.AmazonSnsSecretKey), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            foreach (var recipient in recipients)
            {
                try
                {
                    if (recipient.DeviceOS == Core.Enumerations.Messaging.DeviceOS.iOS)
                    {
                        // format our message for apple
                        client.Publish(new AmazonSNS.Model.PublishRequest() { TargetArn = recipient.ProviderEndpoint, Message = message.MessageSubject }
                        );
                    }
                    else if (recipient.DeviceOS == Core.Enumerations.Messaging.DeviceOS.Android)
                    {
						client.Publish(new AmazonSNS.Model.PublishRequest()
						{
							TargetArn = recipient.ProviderEndpoint,
							MessageStructure = "json",
							Message = string.Format("{{\n\"GCM\": \"{{ \\\"data\\\": {{ \\\"message\\\": \\\"{0}\\\" }} }}\"\n}}", message.MessageSubject)
						});
						
                    }
                }
                catch (Exception ex)
                {
                    eventLog.WriteErrorLog("Error sending message to push recipient: " + recipient.ProviderEndpoint, ex);
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
    }
}
