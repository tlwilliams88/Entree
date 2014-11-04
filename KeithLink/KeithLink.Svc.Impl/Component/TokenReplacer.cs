using KeithLink.Svc.Core.Interface.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Component
{
	public class TokenReplacer: ITokenReplacer
	{
		Regex tokenPattern = new Regex(@"\$\{([a-zA-Z-_ ]*.[0-9a-zA-Z-_ ]*)\}");
		
		public TokenReplacer() { }
		//public TokenReplacer(string tokenValuesFilePath)
		//{
		//	this.tokenValuesCollection = new TokenValuesCollection(tokenValuesFilePath);
		//}

		//public string EvaluateToken(string token)
		//{
		//	if (this.tokenValuesCollection == null)
		//		return null;
		//	return this.tokenValuesCollection[token];
		//}

		public string ReplaceTokens(string template, object valueObject)
		{
			// evaluate type
			PropertyInfo[] valueObjectProperties = new PropertyInfo[] { };
			if (valueObject != null)
				valueObjectProperties = valueObject.GetType().GetProperties();

			var tokenValues = new Dictionary<string, string>();
			foreach (var prop in valueObjectProperties)
				tokenValues.Add(prop.Name, prop.GetValue(valueObject, new object[] { }).ToString());

			return this.ReplaceTokens(template, tokenValues);
		}

		public string ReplaceTokens(string template, Dictionary<string, string> tokenValues)
		{
			if (tokenValues == null)
				tokenValues = new Dictionary<string, string>();

			// match tokens and remove duplicates
			MatchCollection tokenMatchCollection = tokenPattern.Matches(template);

			List<Match> uniqueTokenList = new List<Match>();
			foreach (Match tokenMatch in tokenMatchCollection)
				if (!uniqueTokenList.Exists(m => m.Value.Equals(tokenMatch.Value)))
					uniqueTokenList.Add(tokenMatch);

			foreach (Match tokenMatch in uniqueTokenList)
			{
				string tokenContents = tokenMatch.Groups[1].Value;
				string token = tokenMatch.Value;

				var value = tokenValues.Where(v => v.Key.Equals(tokenContents, StringComparison.OrdinalIgnoreCase)).Select(v => v.Value).FirstOrDefault();

				if (value != null)
					template = template.Replace(token, value);
				//else if (this.EvaluateToken(tokenContents) != null)
				//	template = template.Replace(token, this.EvaluateToken(tokenContents));
			}

			return template;
		}
	}
}
