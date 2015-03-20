using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
	public static class ConfigurationHelper
	{

		public static string GetActiveConfiguration()
		{
#if DEMO
			return "Demo";
#elif TEST
			return "QA";
#elif RELEASE
			return "Production";

#elif STAGE
			return "Staging";
#elif DEV
			return "Dev";
#elif DEBUG
			return "Local Development";
#endif
		}
	}
}
