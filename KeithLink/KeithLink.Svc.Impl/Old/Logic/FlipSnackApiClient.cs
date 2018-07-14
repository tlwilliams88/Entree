using KeithLink.Common.Core;
using KeithLink.Common.Core.Interfaces.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly string _url;
        private readonly string _apiKey;
        private readonly string _secretKey;

        #endregion

        #region ctor
        public FlipSnackApiClient(
            IEventLogRepository eventLogRepository 
            )
        {
            _log = eventLogRepository;

            _url = "http://api.flipsnack.com/v1/";
            _apiKey = ">>apiKey<<";
            _secretKey = ">>secretKey<<";
        }
        #endregion

        #region methods

        public dynamic GetList()
        {
            var requestParameters = new Dictionary<string, string>();

            requestParameters.Add("action", "collection.getList");
            requestParameters.Add("apiKey", _apiKey);
            requestParameters.Add("format", "json");
            requestParameters.Add("orderBy", "name");
            requestParameters.Add("orderMode", "asc");
            requestParameters.Add("query", "");

            var orderedParamenters = requestParameters.OrderBy(param => param.Key).ToList();

            var signatureSeed = new StringBuilder();
            signatureSeed.Append(_secretKey);
            orderedParamenters.ForEach(param => signatureSeed.Append(param.Key + param.Value));
            string signature = Crypto.CalculateMD5Hash(signatureSeed.ToString());

            requestParameters.Add("signature", signature);

            HttpContent requestContent = new FormUrlEncodedContent(requestParameters);
            HttpClient client = new HttpClient();

            dynamic apiResponse = null;

            try
            {
                var httpResponse = client.PostAsync(_url, requestContent).Result;

                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<JObject>(responseContent);
                    string code = apiResponse.code;
                    string status = apiResponse.status;
                    dynamic data = apiResponse.data;

                    if (status == "OK" && data != null)
                    {
                        string collectionCount = data.collectionCount;
                        dynamic[] collections = data.collections;

                        foreach (var collection in collections)
                        {
                            string collectionHash = collection.collectionHash;
                            string collectionTitle = collection.collectionTitle;
                        }
                    }
                    else
                    {
                        string message = string.Format("There was an error invoking GetList in the FlipSnack API. {0}", apiResponse.status);
                        _log.WriteErrorLog(message);
                    }
                }
                else
                {
                    string message = string.Format("There was an error invoking GetList in the FlipSnack API. {0}", httpResponse.ReasonPhrase);
                    _log.WriteErrorLog(message);
                }
            }
            catch (HttpRequestException ex)
            {
                string message = string.Format("There was an error invoking GetList in the FlipSnack API.");
                _log.WriteErrorLog(message, ex);
            }

            return apiResponse;
        }

        public dynamic GetEmbed()
        {
            var requestParameters = new Dictionary<string, string>();

            requestParameters.Add("action", "collection.getEmbed");
            requestParameters.Add("apiKey", _apiKey);
            requestParameters.Add("collectionHash", "");
            requestParameters.Add("format", "json");
            requestParameters.Add("startBook", "1");
            requestParameters.Add("startPage", "1");
            requestParameters.Add("responsive", "true");
            requestParameters.Add("height", "200");
            requestParameters.Add("width", "200");

            var orderedParamenters = requestParameters.OrderBy(param => param.Key).ToList();

            var signatureSeed = new StringBuilder();
            signatureSeed.Append(_secretKey);
            orderedParamenters.ForEach(param => signatureSeed.Append(param.Key + param.Value));
            string signature = Crypto.CalculateMD5Hash(signatureSeed.ToString());

            requestParameters.Add("signature", signature);

            HttpContent requestContent = new FormUrlEncodedContent(requestParameters);
            HttpClient client = new HttpClient();

            dynamic apiResponse = null;

            try
            {
                var httpResponse = client.PostAsync(_url, requestContent).Result;

                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<JObject>(responseContent);
                    string code = apiResponse.code;
                    string status = apiResponse.status;
                    dynamic data = apiResponse.data;

                    if (status == "OK" && data != null)
                    {
                        string embedCode = data.embedCode;
                    }
                    else
                    {
                        string message = string.Format("There was an error invoking GetEmbed in the FlipSnack API. {0}", apiResponse.status);
                        _log.WriteErrorLog(message);
                    }
                }
                else
                {
                    string message = string.Format("There was an error invoking GetEmbed in the FlipSnack API. {0}", httpResponse.ReasonPhrase);
                    _log.WriteErrorLog(message);
                }
            }
            catch (HttpRequestException ex)
            {
                string message = string.Format("There was an error invoking GetEmbed in the FlipSnack API.");
                _log.WriteErrorLog(message, ex);
            }

            return apiResponse;
        }
        #endregion
    }
}
