using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;
using KeithLink.Svc.Core;
using KeithLink.Svc.Impl.Seams;

namespace KeithLink.Svc.Impl.Helpers
{
    class ElasticSearch
    {
        #region " attributes "

        private ElasticLowLevelClient _client;
        private Uri _elasticSearchUri;
        private ConnectionSettings _connectionConfig;


        #endregion

        #region " constructor "

        public ElasticSearch()
        {
            _elasticSearchUri = new Uri(BEKConfiguration.Get("ElasticSearchURL"));
            _connectionConfig = new ConnectionSettings(_elasticSearchUri);
            _client = new ElasticLowLevelClient(_connectionConfig);

        }

        #endregion

        #region " methods / functions "

        public ElasticsearchResponse<DynamicResponse> RawSearch(string index, string type, object filter)
        {
            ElasticsearchResponse<DynamicResponse> response = _client.Search<DynamicResponse>(index, type, filter);
            return response;
        }

        #endregion

        #region " properties "

            public ElasticClient Client
            {
                get { return _client; }
            }

        #endregion

    }
}
