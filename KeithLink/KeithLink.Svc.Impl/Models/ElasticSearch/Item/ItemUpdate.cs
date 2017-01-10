using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KeithLink.Svc.Core.Interface.ElasticSearch;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class ItemUpdate : IESItem
    {
        public RootData index { get; set; } // the variable name of the rootdata object equates to the action; index will handle both create or update

        public string ToJson()
        {
            return string.Format("{0}\n{1}\n", JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(this.index.data));
        }
    }
}
