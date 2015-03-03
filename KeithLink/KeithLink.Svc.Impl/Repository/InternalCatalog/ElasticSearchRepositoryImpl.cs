using KeithLink.Svc.Core.Interface.InternalCatalog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.InternalCatalog
{
    public class ElasticSearchRepositoryImpl: IElasticSearchRepository
    {
        private RestClient client;

        public ElasticSearchRepositoryImpl()
        {
            client = new RestClient(Configuration.ElasticSearchURL);
        }

        public void Create(string json)
        {
            var request = new RestRequest("_bulk", Method.POST);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            client.Execute(request);
        }

		public void MapProductProperties(string branchId, string json)
		{
			var request = new RestRequest(string.Format("{0}/product/_mapping", branchId), Method.POST);
			request.AddParameter("application/json", json, ParameterType.RequestBody);
			client.Execute(request);
		}
		
		public void DeleteBranch(string branchId)
		{
			var request = new RestRequest(branchId, Method.DELETE);
			client.Execute(request);
		}


		public bool CheckIfIndexExist(string branchId)
		{
			var request = new RestRequest(branchId.ToLower(), Method.HEAD);
			var response = client.Execute(request);
			return response == null ? false : response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public void CreateEmptyIndex(string branchId)
		{
			var request = new RestRequest(branchId, Method.PUT);
			//System.Dynamic.ExpandoObject snowballAnalyzer = new System.Dynamic.ExpandoObject();
			//(snowballAnalyzer as IDictionary<string, object>).Add("default", new { type = "custom", filter = new List<string>(){ "standard", "lowercase", "snowball", "my_synonym_filter" }, tokenizer = "standar", language = "English" } );
			//dynamic indexSettings = 
			//	new { settings = 
			//		new { 
			//			index = 
			//			new { number_of_replicas = 1, number_of_shards = 1, analysis =
			//				new { analyzer = 
			//					snowballAnalyzer
			//				}
			//			}
			//		}
			//	};

			dynamic filterDynamic = new { my_synonym_filter = new { type = "synonym", synonyms_path = "synonyms.txt" } };
			System.Dynamic.ExpandoObject dynamicAnalyzer = new System.Dynamic.ExpandoObject();
			(dynamicAnalyzer as IDictionary<string, object>).Add("default", new { type = "custom", filter = new List<string>() { "lowercase", "snowball", "my_synonym_filter" }, tokenizer = "standard" });


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
									filter = filterDynamic,
									analyzer = dynamicAnalyzer
								}
							}
						}
				};

            request.RequestFormat = DataFormat.Json;
            request.AddBody(indexSettings);
			IRestResponse res = client.Execute(request);
		}
	}
}
