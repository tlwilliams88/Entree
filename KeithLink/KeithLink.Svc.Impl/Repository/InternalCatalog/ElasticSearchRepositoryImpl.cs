using Elasticsearch.Net;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Models.ElasticSearch.Item;
using KeithLink.Svc.Impl.Models.ElasticSearch.Item;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.InternalCatalog {
    /// <summary>
    /// Specific implementation for dealing with the elasticsearch api
    /// </summary>
    public class ElasticSearchRepositoryImpl: IElasticSearchRepository {
        #region attributes
        private RestClient client;
        #endregion

        #region ctor
        public ElasticSearchRepositoryImpl()
        {
            client = new RestClient(Configuration.ElasticSearchURL);
        }
        #endregion

        #region methods
		public bool CheckIfIndexExist(string branchId)
		{
			var request = new RestRequest(branchId.ToLower(), Method.HEAD);
			var response = client.Execute(request);
			return response == null ? false : response.StatusCode == System.Net.HttpStatusCode.OK;
		}

        public void Create(string json)
        {
            var request = new RestRequest("_bulk", Method.POST);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = client.Execute(request);
        }

        public void CreateEmptyIndex(string branchId) {
            dynamic filters = new {
                my_synonym_filter = new { type = "synonym", synonyms_path = "synonyms.txt", ignore_case = "true" },
                ngram_filter = new { type = "ngram", min_gram = 2, max_gram = 20, token_chars = new[] { "letter", "digit" } }
            };

            // trying to convert this to a dynamic object fails. It causes the name_ngram_analyzed column
            // to lose its index_analyzer and search_analyzer settings.
            System.Dynamic.ExpandoObject dynamicAnalyzer = new System.Dynamic.ExpandoObject();
            (dynamicAnalyzer as IDictionary<string, object>).Add("default", new {
                                                                                type = "custom",
                                                                                filter = new List<string>() {
                                                                                        "standard", 
                                                                                        "lowercase",
                                                                                        "my_synonym_filter", 
                                                                                    },
                                                                                    tokenizer = "whitespace"
                                                                                }
                                                                            );
            (dynamicAnalyzer as IDictionary<string, object>).Add("whitespace_analyzer", new {
                                                                                    type = "custom",
                                                                                    filter = new List<string>() { "lowercase", "my_synonym_filter" },
                                                                                    tokenizer = "whitespace"
                                                                                }
                                                                            );
            (dynamicAnalyzer as IDictionary<string, object>).Add("ngram_analyzer", new {
                                                                                    type = "custom",
                                                                                    filter = new List<string>() {
                                                                                        "lowercase",
                                                                                        "ngram_filter",
                                                                                        "my_synonym_filter"
                                                                                    },
                                                                                    tokenizer = "whitespace"
                                                                                }
                                                                            );
            dynamic indexSettings =
                new {
                    settings =
                        new {
                            index =
                            new {
                                number_of_replicas = 1,
                                number_of_shards = 1,
                                analysis = new {
                                    filter = filters,
                                    analyzer = dynamicAnalyzer
                                }
                            }
                        }
                };

            var test = Newtonsoft.Json.JsonConvert.SerializeObject(indexSettings);

            var request = new RestRequest(branchId, Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(indexSettings);
            IRestResponse res = client.Execute(request);
        }
 
        public void DeleteBranch(string branchId)
		{
			var request = new RestRequest(branchId, Method.DELETE);
			client.Execute(request);
		}

        private dynamic BuildBranchMetaQuery(int size)
        {
            return new
            {
                size = size,
                query = new
                {
                    match_all = new { }
                },
                fields = new string[0]
            };
        }

        private dynamic BuildScrollQuery(string scrollName, string scrollId)
        {
            return new
            {
                scroll = scrollName,
                scroll_id = scrollId
            };
        }

        private ElasticsearchClient GetElasticsearchClient()
        {
            var node = new Uri(Configuration.ElasticSearchURL);
            var config = new Elasticsearch.Net.Connection.ConnectionConfiguration(node);
            var client = new ElasticsearchClient(config);
            return client;
        }

        /// <summary>
        /// Method for reading all the meta information from the documents within product type within the specified branch
        /// and return a list of the string ids of those documents
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<string> ReadListOfProductsByBranch(string branchId)
        {
            int elasticSearchCursor = 0;

            string json = JsonConvert.SerializeObject(BuildBranchMetaQuery(Configuration.ElasticSearchBatchSize));

            string scroll = "1m"; //the scroll api is used to read a large dataset, this is the scroll name

            var request = new RestRequest(string.Format("{0}/product/_search?scroll={1}", branchId, scroll), Method.POST);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var res = client.Execute(request);
            DynamicDictionary dd = JsonConvert.DeserializeObject<DynamicDictionary>(res.Content);

            int totalproducts = dd["hits"]["total"];

            string scrollid = dd["_scroll_id"]; //this scroll id is read in the first time and then is passed into subsequent calls

            dynamic Ids = dd["hits"]["hits"];

            List<string> existing = new List<string>();

            foreach(var Id in Ids)
            {
                existing.Add(Id._id.ToString());
            }

            elasticSearchCursor += Configuration.ElasticSearchBatchSize;

            while (elasticSearchCursor < totalproducts)
            {
                json = JsonConvert.SerializeObject(BuildScrollQuery(scroll, scrollid));

                request = new RestRequest("_search/scroll", Method.POST);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                res = client.Execute(request);
                dd = JsonConvert.DeserializeObject<DynamicDictionary>(res.Content);

                Ids = dd["hits"]["hits"];

                if(Ids != null)
                {
                    foreach (var Id in Ids)
                    {
                        if (Id != null && Id._id != null && existing.Contains(Id._id.ToString()) == false)
                        {
                            existing.Add(Id._id.ToString());
                        }
                    }
                }

                elasticSearchCursor += Configuration.ElasticSearchBatchSize;
            }

            return existing;
        }

        public void MapProductProperties(string branchId, string json)
		{
			var request = new RestRequest(string.Format("{0}/product/_mapping", branchId), Method.POST);
			request.AddParameter("application/json", json, ParameterType.RequestBody);
			client.Execute(request);
		}
		
		public void RefreshSynonyms(string branchId)
		{
			//Close then open the index. This will cause it to see any changes made to the synonyms file
			var requestClose = new RestRequest(string.Format("{0}/_close",branchId.ToLower()), Method.POST);
			client.Execute(requestClose);

			var requestOpen = new RestRequest(string.Format("{0}/_open", branchId.ToLower()), Method.POST);
			client.Execute(requestOpen);
		}
        #endregion
	}
}
