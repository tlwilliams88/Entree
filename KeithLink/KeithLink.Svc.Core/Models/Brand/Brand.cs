using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Nest;

namespace KeithLink.Svc.Core.Models.Brand
{
    
    [DataContract(Name="brand")]
    [ElasticType(Name="brand")]
    public class Brand
    {
        #region " attributes "
        #endregion

        #region " constructor"
       
        #endregion

        #region " methods / functions "
        #endregion

        #region " properties "

        [DataMember(Name="id")]
        [ElasticProperty(Name="id")]
        public string Id {get;set;}
        [DataMember(Name="name")]
        [ElasticProperty(Name="name")]
		public string Name { get; set; }
        [DataMember(Name="imageurl")]
        [ElasticProperty(Name="imageurl")]
		public string ImageURL { get; set; }
        #endregion
    }
}
