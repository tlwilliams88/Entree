using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Component
{
	public interface ITokenReplacer
	{
		string ReplaceTokens(string template, object valueObject);
		string ReplaceTokens(string template, Dictionary<string, string> tokenValues);
	}
}
