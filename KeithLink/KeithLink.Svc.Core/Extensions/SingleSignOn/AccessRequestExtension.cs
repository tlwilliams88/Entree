using KeithLink.Svc.Core.Models.SingleSignOn;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KeithLink.Svc.Core.Extensions.SingleSignOn {
    public static class AccessRequestExtension {
        public static string ToJSON(this BaseAccessRequest AccessRequest) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(AccessRequest.GetType());

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                serializer.WriteObject(ms, AccessRequest);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static BaseAccessRequest Deserialize(this string json) {
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(json))) {
                var serializer = new DataContractJsonSerializer(typeof(BaseAccessRequest));
                BaseAccessRequest request = (BaseAccessRequest)serializer.ReadObject(ms);

                ms.Position = 0;

                switch (request.RequestType) {
                    case Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer:
                        return (BaseAccessRequest)new DataContractJsonSerializer(typeof(KbitCustomerAccessRequest)).ReadObject(ms);
                    case Core.Enumerations.SingleSignOn.AccessRequestType.PowerMenu:
                        return (BaseAccessRequest)new DataContractJsonSerializer( typeof( PowerMenuCustomerAccessRequest ) ).ReadObject( ms );
                    default:
                        return request;
                }
            }
        }
    }
}
