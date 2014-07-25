using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models
{
    public class ElasticSearchCategoryUpdate
    {
        public ESCategoryRootData index { get; set; }
        
        public string ToJson()
        {
            return string.Format("{0}\n{1}\n", JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(this.index.data));
        }
    }

    public class ESCategoryRootData
    {
        public string _index { get { return "categories"; } }
        public string _type { get { return "category"; } }
        public string _id { get; set; }
        [JsonIgnore]
        public ESCategoryData data { get; set; }
    }

    public class ESCategoryData
    {
        public string parentcategoryid { get; set; }
        public string name { get; set; }
        public string ppicode { get; set; }
        public List<ESSubCategories> subcategories { get; set; }
    }
    
    public class ESSubCategories
    {
        public string categoryid { get; set; }
        public string name { get; set; }
        public string ppicode { get; set; }
    }

}
