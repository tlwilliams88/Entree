using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KeithLink.Svc.Core.Interface.ElasticSearch;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class ItemDelete : ItemBase, IESItem
    {
        public RootData delete { get; set; } // The rootdata variable name is serialized into the action during the ES load process
                                             // the "delete" action serves to delete an existing ES document during the batch load of elasticsearch

        public new string ToJson() // a specialized ToJson is needed for this type of variable
        {
            return string.Format("{0}\n", JsonConvert.SerializeObject(this));
        }
    }
}
