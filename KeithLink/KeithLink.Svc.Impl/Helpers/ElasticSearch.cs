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

        private ElasticClient _client;
        private Uri _elasticSearchUri;
        private ConnectionSettings _connectionConfig;


        #endregion

        #region " constructor "

        public ElasticSearch()
        {
            _elasticSearchUri = new Uri(BEKConfiguration.Get("ElasticSearchURL"));
            _connectionConfig = new ConnectionSettings(_elasticSearchUri);
            _client = new ElasticClient(_connectionConfig);
        }

        #endregion

        #region " methods / functions "
        #endregion

        #region " properties "

        public ElasticClient ElasticClient {
            get { return _client; }
        }

        #endregion

    }
}
