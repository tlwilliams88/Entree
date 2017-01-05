﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KeithLink.Svc.Core.Interface.ElasticSearch;

namespace KeithLink.Svc.Impl.Models.ElasticSearch.Item
{
    public class ItemInsert : IESItem
    {
        public RootData index { get; set; }

        public string ToJson()
        {
            return string.Format("{0}\n{1}\n", JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(this.index.data));
        }
    }
}
