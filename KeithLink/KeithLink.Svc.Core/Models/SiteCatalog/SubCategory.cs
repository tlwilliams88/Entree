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
        private StringBuilder _id;
        private StringBuilder _name;
        private StringBuilder _description;
        private StringBuilder _ppicode;
        #endregion

        #region " constructor "
        public SubCategory()
        {
            _id = new StringBuilder();
            _name = new StringBuilder();
            _description = new StringBuilder();
            _ppicode = new StringBuilder();
        }
        #endregion

        #region " methods/functions "
        #endregion

        #region " properties "
        [ElasticProperty(Name="categoryid")]
        [DataMember(Name = "categoryid")]
        public string Id
        {
            get
            {
                return _id.ToString();
            }
            set
            {
                _id = new StringBuilder(value);
            }
        }

        [ElasticProperty(Name = "name")]
        [DataMember(Name = "name")]
        public string Name
        {
            get
            {
                return _name.ToString();
            }
            set
            {
                _name = new StringBuilder(value);
            }
        }

        [ElasticProperty(Name = "description")]
        [DataMember(Name = "description")]
        public string Description
        {
            get
            {
                return _description.ToString();
            }
            set
            {
                _description = new StringBuilder(value);
            }
        }

        [ElasticProperty(Name = "ppicode")]
        [DataMember(Name = "ppicode")]
        public string PPICode
        {
            get
            {
                return _ppicode.ToString();
            }
            set
            {
                _ppicode = new StringBuilder(value);
            }
        }
        #endregion
    }
}
