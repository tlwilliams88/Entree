using Entree.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.SiteCatalog
{
	public interface IDivisionRepository
	{
		List<Catalog> GetDivisions();
	}
}
