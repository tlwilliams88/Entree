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
			client.Execute(request);
		}
	}
}
