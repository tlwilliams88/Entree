using KeithLink.Svc.Core.Interface.InternalCatalog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.InternalCatalog {
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
            client.Execute(request);
        }

		public void CreateEmptyIndex(string branchId) {
			var request = new RestRequest(branchId, Method.PUT);
            //dynamic synonymFilter = new { my_synonym_filter = new { type = "synonym", synonyms_path = "synonyms.txt" } };
            //dynamic autoCompleteFilter = new { autcomplete_filter = new {type = "edge_ngram", min_gram = 2, max_gram = 20 }};

            dynamic filters = new {
                my_synonym_filter = new { type = "synonym", synonyms_path = "synonyms.txt" },
                autcomplete_filter = new { type = "ngram", min_gram = 2, max_gram = 20 }
            };

            dynamic edge_ngram_tokenizer = new { my_edge_ngram_tokenizer = new { type = "edgeNGram", min_gram = 2, max_gram = 20, token_chars = new[] { "letter", "digit" } } };

            System.Dynamic.ExpandoObject dynamicAnalyzer = new System.Dynamic.ExpandoObject();
			(dynamicAnalyzer as IDictionary<string, object>).Add("default", new { 
                                                                                    type = "custom", 
                                                                                    filter = new List<string>() {
                                                                                        "my_synonym_filter", 
                                                                                        //"autcomplete_filter",
                                                                                        "standard", 
                                                                                        "lowercase"
                                                                                    }, 
                                                                                    tokenizer = "whitespace" 
                                                                                }
                                                                            );
			(dynamicAnalyzer as IDictionary<string, object>).Add("my_edge_ngram_analyzer", new { 
                                                                                    type = "custom",
                                                                                    filter = new List<string>() {
                                                                                        "autcomplete_filter",
                                                                                        "lowercase"
                                                                                    }, 
                                                                                    tokenizer = "my_edge_ngram_tokenizer" 
                                                                                }
                                                                            );
			dynamic indexSettings =
				new
				{
					settings =
						new
						{
							index =
							new
							{
								number_of_replicas = 1,
								number_of_shards = 1,
								analysis = new
								{
                                    filter = filters,
									analyzer = dynamicAnalyzer
								},
                                tokenizer = edge_ngram_tokenizer
							}
						}
				};

			var test = Newtonsoft.Json.JsonConvert.SerializeObject(indexSettings);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(indexSettings);
			IRestResponse res = client.Execute(request);
		}
		
        public void DeleteBranch(string branchId)
		{
			var request = new RestRequest(branchId, Method.DELETE);
			client.Execute(request);
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
