﻿// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ETL.ElasticSearch {
    public interface IItemImport {
        void ImportItems();
		void ImportUNFIItems();
        void ImportUNFIEastItems();
    }
}
