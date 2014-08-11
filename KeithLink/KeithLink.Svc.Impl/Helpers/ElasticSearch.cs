using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl.Helpers
{
    class ElasticSearch
    {
        #region " attributes "

        private ElasticClient _client;
        private Uri _elasticSearchUri;
        private ConnectionSettings _connectionConfig;


        #endregion

        #region " constructor "

        public ElasticSearch()
        {
            _elasticSearchUri = new Uri(Configuration.ElasticSearchURL);
            _connectionConfig = new ConnectionSettings(_elasticSearchUri);
            _client = new ElasticClient(_connectionConfig);

        }

        #endregion

        #region " methods / functions "

        public ElasticsearchResponse<DynamicDictionary> RawSearch(string index, string type, object filter)
        {
            ElasticsearchResponse<DynamicDictionary> response = _client.Raw.Search(index, type, filter);
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
