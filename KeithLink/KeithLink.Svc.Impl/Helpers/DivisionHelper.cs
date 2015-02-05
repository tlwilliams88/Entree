using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
	public static class DivisionHelper
	{
		public  static string GetDivisionFromBranchId(string branchId)
		{
			if (branchId.Length == 5)
			{
				return branchId;
			}
			else if (branchId.Length == 3)
			{
				switch (branchId.ToUpper())
				{
					case "FAM":
						return "FAM04";
					case "FAQ":
						return "FAQ08";
					case "FAR":
						return "FAR09";
					case "FDF":
						return "FDF01";
					case "FHS":
						return "FHS03";
					case "FLR":
						return "FLR05";
					case "FOK":
						return "FOK06";
					case "FSA":
						return "FSA07";
					default:
						return null;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
