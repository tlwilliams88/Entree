using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Nest;


namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name="category")]
    public class SubCategory
    {
        #region " attributes "
         #endregion

        #region " constructor "
        #endregion

        #region " methods/functions "
        #endregion

        #region " properties "
        [ElasticProperty(Name="categoryid")]
        [DataMember(Name = "categoryid")]
		public string Id { get; set; }

        [ElasticProperty(Name = "name")]
        [DataMember(Name = "name")]
		public string Name { get; set; }

        [ElasticProperty(Name = "search_name")]
        [DataMember(Name = "search_name")]
        public string SearchName { get; set; }

        [ElasticProperty(Name = "description")]
        [DataMember(Name = "description")]
		public string Description { get; set; }

        [ElasticProperty(Name = "ppicode")]
        [DataMember(Name = "ppicode")]
		public string PPICode { get; set; }
        #endregion
    }
}
