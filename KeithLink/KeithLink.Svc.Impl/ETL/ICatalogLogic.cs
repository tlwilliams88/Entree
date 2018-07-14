using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.ETL
{
    public interface ICatalogLogic
    {
        void ImportCatalog();
		void ImportUNFICatalog();
        void ImportUNFIEastCatalog();
    }
}
