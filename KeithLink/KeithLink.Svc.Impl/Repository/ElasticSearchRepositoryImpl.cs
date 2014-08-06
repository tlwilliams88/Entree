using KeithLink.Svc.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository
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
            var response = client.Execute(request);
        }
    }
}
