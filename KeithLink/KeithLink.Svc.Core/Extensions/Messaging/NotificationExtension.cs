using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using KeithLink.Svc.Core.Models.Messaging.Queue;

namespace KeithLink.Svc.Core.Extensions.Messaging
{
    public static class NotificationExtension
    {
        public static string ToJson(this BaseNotification baseNotification)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(baseNotification.GetType());

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, baseNotification);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static BaseNotification Deserialize(string json)
        {
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(BaseNotification));
                BaseNotification notification = (BaseNotification)serializer.ReadObject(ms);

                ms.Position = 0;

                switch (notification.NotificationType) {
                    case Core.Enumerations.Messaging.NotificationType.OrderConfirmation:
                        return (BaseNotification)new DataContractJsonSerializer(typeof(OrderConfirmationNotification)).ReadObject(ms);    
                    case Core.Enumerations.Messaging.NotificationType.Eta:
                        return (BaseNotification)new DataContractJsonSerializer(typeof(EtaNotification)).ReadObject(ms);
                    case Core.Enumerations.Messaging.NotificationType.PaymentConfirmation:
                        return (BaseNotification)new DataContractJsonSerializer(typeof(PaymentConfirmationNotification)).ReadObject(ms);
					case Core.Enumerations.Messaging.NotificationType.HasNews:
						return (BaseNotification)new DataContractJsonSerializer(typeof(HasNewsNotification)).ReadObject(ms);
                    default:
                        return notification;
                }
            }
        }
    }
}
