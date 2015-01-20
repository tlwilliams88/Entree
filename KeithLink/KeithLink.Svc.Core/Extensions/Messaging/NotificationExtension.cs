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
                if (notification.NotificationType == Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                {
                    ms.Position = 0;
                    return (BaseNotification)new DataContractJsonSerializer(typeof(OrderConfirmationNotification)).ReadObject(ms);
                }
                else if (notification.NotificationType == Core.Enumerations.Messaging.NotificationType.Eta)
                {
                    ms.Position = 0;
                    return (BaseNotification)new DataContractJsonSerializer(typeof(EtaNotification)).ReadObject(ms);
                }
                return notification;
            }
        }
    }
}
