using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public abstract class ItemBase
    {
        public RootData index { get; set; } // The rootdata variable name is serialized into the action during the ES load process
                                            // the "index" action serves both to create the ES document and replace an existing document 
                                            // during the batch load of elasticsearch

        public string ToJson()
        {
            return string.Format("{0}\n{1}\n", JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(this.index.data));
        }
    }
}
