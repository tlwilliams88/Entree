using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace KeithLink.Svc.Impl.Logic
{
    public class FlipSnackApiClient
    {
        #region attributes
        private readonly IEventLogRepository _log;

        private const string MESSAGE_TEMPLATE_FORWARDUSERMESSAGE = "ForwardUserMessage";
        #endregion

        #region ctor
        public FlipSnackApiClient(
            IEventLogRepository eventLogRepository 
            )
        {
            _log = eventLogRepository;
        }
        #endregion

        #region methods

        public string GetEmbed()
        {
            string apiKey = "";
            string signature = "";
            string action = "collection.getEmbed";
            string collectionHash = "";
            string format = "json";
            bool responsive = true;
            int startPage = 1;
            int startBook = 1;
            int width = 200;
            int height = 200;


            var request = new StringBuilder();
            request.Append("apiKey=" + apiKey);
            request.Append("&signature =" + signature);
            request.Append("&action=" + action);
            request.Append("&collectionHash =" + collectionHash);
            request.Append("&format=" + format);
            request.Append("&responsive=" + responsive);
            request.Append("&startPage=" + startPage);
            request.Append("&startBook=" + startBook);
            request.Append("&width=" + width);
            request.Append("&height=" + height);
            HttpContent requestContent = new StringContent(request.ToString());
            HttpClient client = new HttpClient();

            var formatter = new FormUrlEncodedMediaTypeFormatter();

            string responseContent = null;

            try
            {
                string url = "http://api.flipsnack.com/v1/";
                var response = client.PostAsync(url, requestContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    responseContent = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    responseContent = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                _log.WriteErrorLog("GetEmbed", ex);
            }

            return responseContent;
        }
        #endregion
    }
}
